using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ReactNativeCmdEase
{
    public class GradleConfig
    {

        private GradleNode m_root;
        private String m_filePath;
        private GradleNode m_curNode;

        public GradleNode ROOT
        {
            get { return m_root; }
        }

        public GradleConfig(string filePath)
        {
            string file = File.ReadAllText(filePath);
            TextReader reader = new StringReader(file);

            m_filePath = filePath;
            m_root = new GradleNode("root");
            m_curNode = m_root;

            StringBuilder str = new StringBuilder();

            while (reader.Peek() > 0)
            {
                char c = (char)reader.Read();
                switch (c)
                {
                    case '/':
                        if (reader.Peek() == '/')
                        {
                            reader.Read();
                            str.Append('\n');
                            string comment = "";
                            bool insidecomment = true;
                            do
                            {
                                char c2 = (char)reader.Read();
                                comment += c2.ToString();
                                if (reader.Peek() == '\n')
                                {
                                    reader.Read();
                                    insidecomment = false;
                                }

                            } while (insidecomment);
                            m_curNode.AppendChildNode(new GradleCommentNode(comment, m_curNode));

                        }
                        else if (reader.Peek() == '*')
                        {
                            reader.Read();
                            bool insidecomment = true;
                            string comment2 = "/*";
                            do
                            {
                                char c2 = (char)reader.Read();
                                comment2 += c2.ToString();
                                if (c2 == '*' && reader.Peek() == '/')
                                {
                                    reader.Read();
                                    comment2 += '/';
                                    m_curNode.AppendChildNode(new GradleContentNode(comment2, m_curNode));
                                    insidecomment = false;
                                }

                            } while (insidecomment);
                        }
                        else
                        {
                            str.Append('/');
                        }
                        break;
                    case '\n':
                        {
                            var strf = FormatStr(str);
                            if (!string.IsNullOrEmpty(strf))
                            {
                                m_curNode.AppendChildNode(new GradleContentNode(strf, m_curNode));
                            }
                        }
                        str = new StringBuilder();
                        break;
                    case '\r':
                        break;
                    case '\t':
                        break;
                    case '{':
                        {
                            var n = FormatStr(str);
                            if (!string.IsNullOrEmpty(n))
                            {
                                GradleNode node = new GradleNode(n, m_curNode);
                                m_curNode.AppendChildNode(node);
                                m_curNode = node;
                            }
                        }
                        str = new StringBuilder();
                        break;
                    case '}':
                        {
                            var strf = FormatStr(str);
                            if (!string.IsNullOrEmpty(strf))
                            {
                                m_curNode.AppendChildNode(new GradleContentNode(strf, m_curNode));
                            }
                            m_curNode = m_curNode.PARENT;
                        }
                        str = new StringBuilder();
                        break;
                    default:
                        str.Append(c);
                        break;
                }
            }

            //Debug.Log("Gradle parse done!");
        }

        public void Save(string path = null)
        {
            if (path == null)
                path = m_filePath;
            File.WriteAllText(path, Print());
            //Debug.Log("Gradle parse done! " + path);
        }

        private string FormatStr(StringBuilder sb)
        {
            string str = sb.ToString();
            str = str.Trim();
            return str;
        }
        public string Print()
        {
            StringBuilder sb = new StringBuilder();
            printNode(sb, m_root, -1);
            return sb.ToString();
        }
        private string GetLevelIndent(int level)
        {
            if (level <= 0) return "";
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < level; i++)
            {
                sb.Append('\t');
            }
            return sb.ToString();
        }
        private void printNode(StringBuilder stringBuilder, GradleNode node, int level)
        {
            if (node.PARENT != null)
            {
                if (node is GradleCommentNode)
                {
                    stringBuilder.Append("\n" + GetLevelIndent(level) + @"//" + node.NAME);
                }
                else
                {
                    stringBuilder.Append("\n" + GetLevelIndent(level) + node.NAME);
                }

            }

            if (!(node is GradleContentNode) && !(node is GradleCommentNode))
            {
                if (node.PARENT != null)
                {
                    stringBuilder.Append(" {");
                }
                foreach (var c in node.CHILDREN)
                {
                    printNode(stringBuilder, c, level + 1);
                }
                if (node.PARENT != null)
                {
                    stringBuilder.Append("\n" + GetLevelIndent(level) + "}");
                }
            }
        }
    }

    public class GradleNode
    {
        [JsonIgnore]
        protected List<GradleNode> m_children = new List<GradleNode>();
        [JsonIgnore]
        protected GradleNode m_parent;
        [JsonIgnore]
        protected String m_name;
        [JsonIgnore]
        public GradleNode PARENT
        {
            get { return m_parent; }
        }

        public string NAME
        {
            get { return m_name; }
        }

        public List<GradleNode> CHILDREN
        {
            get { return m_children; }
        }

        public GradleNode(string name, GradleNode parent = null)
        {
            m_parent = parent;
            m_name = name;
        }

        public void Each(Action<GradleNode> f)
        {
            f(this);
            foreach (var n in m_children)
            {
                n.Each(f);
            }
        }

        public void AppendChildNode(GradleNode node)
        {
            if (m_children == null) m_children = new List<GradleNode>();
            m_children.Add(node);
            node.m_parent = this;
        }

        public GradleNode TryGetNode(string path)
        {
            string[] subpath = path.Split('/');
            GradleNode cnode = this;
            for (int i = 0; i < subpath.Length; i++)
            {
                var p = subpath[i];
                if (string.IsNullOrEmpty(p)) continue;
                GradleNode tnode = cnode.FindChildNodeByName(p);
                if (tnode == null)
                {
                    //Debug.Log("Can't find Node:" + p);
                    return null;
                }

                cnode = tnode;
                tnode = null;
            }

            return cnode;
        }

        public GradleNode FindChildNodeByName(string name)
        {
            foreach (var n in m_children)
            {
                if (n is GradleCommentNode || n is GradleContentNode)
                    continue;
                if (n.NAME == name)
                    return n;
            }
            return null;
        }

        public bool ReplaceContenStartsWith(string patten, string value)
        {
            foreach (var n in m_children)
            {
                if (!(n is GradleContentNode)) continue;
                if (n.m_name.StartsWith(patten))
                {
                    n.m_name = value;
                    return true;
                }
            }
            return false;
        }

        public GradleContentNode ReplaceContenOrAddStartsWith(string patten, string value)
        {
            foreach (var n in m_children)
            {
                if (!(n is GradleContentNode)) continue;
                if (n.m_name.StartsWith(patten))
                {
                    n.m_name = value;
                    return (GradleContentNode)n;
                }
            }
            return AppendContentNode(value);
        }

        public GradleContentNode AppendContentNode(string content)
        {
            foreach (var n in m_children)
            {
                if (!(n is GradleContentNode)) continue;
                if (n.m_name == content)
                {
                    //Debug.Log("GradleContentNode with " + content + " already exists!");
                    return null;
                }
            }
            GradleContentNode cnode = new GradleContentNode(content, this);
            AppendChildNode(cnode);
            return cnode;
        }


        public bool RemoveContentNode(string contentPattern)
        {
            for (int i = 0; i < m_children.Count; i++)
            {
                if (!(m_children[i] is GradleContentNode)) continue;
                if (m_children[i].m_name.Contains(contentPattern))
                {
                    m_children.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }

    public sealed class GradleContentNode : GradleNode
    {
        public GradleContentNode(String content, GradleNode parent) : base(content, parent)
        {

        }

        public void SetContent(string content)
        {
            m_name = content;
        }
    }

    public sealed class GradleCommentNode : GradleNode
    {
        public GradleCommentNode(String content, GradleNode parent) : base(content, parent)
        {

        }

        public string GetComment()
        {
            return m_name;
        }
    }
}

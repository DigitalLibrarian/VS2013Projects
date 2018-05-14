using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Math
{
    public class TreeNode<T>
    {
        public List<TreeNode<T>> Children { get; private set; }
        public T Object { get; set; }
        public bool IsLeaf { get { return Children.Count() == 0; } }

        public TreeNode(T obj, List<TreeNode<T>> children)
        {
            Object = obj;
            Children = children;
        }

        public TreeNode(T obj)
            : this(obj, new List<TreeNode<T>>())
        {

        }
    }
}

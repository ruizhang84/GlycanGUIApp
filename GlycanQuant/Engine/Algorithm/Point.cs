using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Model.Algorithm
{
    public class Point<T> : IComparable<Point<T>>
    {
        protected double value;
        protected T content;
        public Point(double value, T content)
        {
            this.value = value;
            this.content = content;
        }
        public double Value() { return value; }
        public void SetValue(double value) { this.value = value; }
        public T Content() { return content; }
        public void SetContent(T content) { this.content = content; }

        public int CompareTo(Point<T> other)
        {
            return value.CompareTo(other.value);
        }
    }
}

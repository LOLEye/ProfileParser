using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileParser.Misc
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class PropertyTagAttribute : Attribute
    {
        public TagType TagType { get; }

        public PropertyTagAttribute(TagType tag = TagType.Required)
        {
            TagType = tag;
        }
    }

    internal enum TagType
    {
        /// <summary>
        /// 可选属性  具有默认值
        /// </summary>
        Optional,
        /// <summary>
        /// 必选属性  如果为空则解析阶段发出错误
        /// </summary>
        Required,
        /// <summary>
        /// 忽视  设置后属性为null;  值类型为0
        /// </summary>
        Ignored
    }
}

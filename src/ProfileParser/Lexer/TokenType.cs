using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileParser.Lexer
{
    public enum TokenType
    {
        /// <summary>
        /// 空格
        /// </summary>
        Blank,
        /// <summary>
        /// 注释
        /// </summary>
        Comment,
        /// <summary>
        /// 单个元素
        /// set
        /// unset
        /// reset
        /// </summary>
        SingleProperty,
        /// <summary>
        /// stage {
        /// }
        /// </summary>
        StageBlock,
        /// <summary>
        /// http-get {
        /// }
        /// </summary>
        HttpGetBlock,
        /// <summary>
        /// http-post {
        /// }
        /// </summary>
        HttpPostBlock,
        /// <summary>
        /// client {
        /// }
        /// </summary>
        ClientBlock,
        /// <summary>
        /// metadata {
        /// }
        /// </summary>
        MetadataBlock,
        /// <summary>
        /// server {
        /// }
        /// </summary>
        ServerBlock,
        /// <summary>
        /// output {
        /// }
        /// </summary>
        OutputBlock,
        /// <summary>
        /// id {
        /// }
        /// </summary>
        IdBlock
    }
}

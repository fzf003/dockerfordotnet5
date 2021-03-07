using AkkaPersistenceConsole.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaPersistenceConsole.Actors
{
    public class Print
    {
        public static Print Instance = new Print();
        private Print() { }
    }
   public class CreateProductEvent
    {
        public long ProductId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>

        public string Description { get; set; }
        /// <summary>
        /// 图片
        /// </summary>

        public string ImageFile { get; set; }
        /// <summary>
        /// 商品价格
        /// </summary>

        public decimal? Price { get; set; }
        /// <summary>
        /// 商品状态
        /// </summary>

        public ProductStatus Status { get; set; }
    }
}

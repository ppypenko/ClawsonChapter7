using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name="P1"},
                new Product {ProductID = 2, Name="P2"},
                new Product {ProductID = 3, Name="P3"},
                new Product {ProductID = 4, Name="P4"},
                new Product {ProductID = 5, Name="P5"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

            Product[] prodarray = result.Products.ToArray();
            Assert.IsTrue(prodarray.Length == 2);
            Assert.AreEqual(prodarray[0].Name, "P4");
            Assert.AreEqual(prodarray[1].Name, "P5");
        }
        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1" },
                new Product {ProductID = 2, Name = "P2" },
                new Product {ProductID = 3, Name = "P3" },
                new Product {ProductID = 4, Name = "P4" },
                new Product {ProductID = 5, Name = "P5" }
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

            PagingInfo pageinfo = result.PagingInfo;
            Assert.AreEqual(pageinfo.CurrentPage, 2);
            Assert.AreEqual(pageinfo.ItemsPerPage, 3);
            Assert.AreEqual(pageinfo.TotalItems, 5);
            Assert.AreEqual(pageinfo.TotalPages, 2);
        }
        [TestMethod]
        public void Can_Filter_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1" },
                new Product {ProductID = 2, Name = "P2", Category = "Cat2" },
                new Product {ProductID = 3, Name = "P3", Category = "Cat1" },
                new Product {ProductID = 4, Name = "P4", Category = "Cat2" },
                new Product {ProductID = 5, Name = "P5", Category = "Cat3" }
            });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            Product[] result = ((ProductListViewModel)controller.List("Cat2", 1).Model)
                .Products.ToArray();

            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }
        [TestMethod]
        public void Can_Create_Categories()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "P1", Category = "Apples" },
                new Product { ProductID = 2, Name = "P2", Category = "Apples" },
                new Product { ProductID = 3, Name = "P3", Category = "Plums" },
                new Product { ProductID = 4, Name = "P4", Category = "Oranges" }
            });

            NavController target = new NavController(mock.Object);

            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();

            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }
        [TestMethod]
        public void Indicate_Selected_Category()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "P1", Category = "Apples" },
                new Product { ProductID = 2, Name = "P2", Category = "Oranges" }
            });
            NavController target = new NavController(mock.Object);
            string categoryToSelect = "Apples";
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;
            Assert.AreEqual(categoryToSelect, result);
        }
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }
        [TestMethod]
        public void Can_Add_For_Existing_Lines()
        {

            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }
        [TestMethod]
        public void Can_Remove_Line()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);
            target.RemoveLine(p2);

            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }
        [TestMethod]
        public void Calculate_Cart_Total()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            Assert.AreEqual(result, 450M);
        }
        [TestMethod]
        public void Can_Clear_Contents()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            target.Clear();

            Assert.AreEqual(target.Lines.Count(), 0);
        }
    }
}

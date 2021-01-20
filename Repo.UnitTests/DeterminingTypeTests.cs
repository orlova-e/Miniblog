using Domain.Entities;
using NUnit.Framework;
using System;

namespace Repo.UnitTests
{
    [TestFixture]
    public class DeterminingTypeTests
    {
        [Test]
        public void Determine_UsingDbTableTypeString_ReturnsDbTableType()
        {
            string type = "Article";

            Type result = DeterminingType.Determine(type);

            Assert.AreEqual(typeof(Article).Name, result.Name);
        }

        [Test]
        public void Determine_WrongType_ThrowsArgumentException()
        {
            string type = "Smth";

            Assert.Throws(typeof(ArgumentException), () => DeterminingType.Determine(type));
        }

        [Test]
        public void Determine_NullType_ThrowsArgumentNullException()
        {
            string type = null;

            Assert.Throws(typeof(ArgumentNullException), () => DeterminingType.Determine(type));
        }
    }
}

using NUnit.Framework;
using Services.Implementation;
using System;
using System.Collections.Generic;

namespace Services.UnitTests
{
    [TestFixture]
    public class InputPreparationTests
    {
        [Test]
        public void PrepareAllKeyboardSymbolsReturnsStringListWithWordFormingSymbols()
        {
            string tested = @"~!@#$%^&*()_+-=`|\/?.,'<>{}[]""QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";

            List<string> expected = new List<string>()
            {
                "_-QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm"
            };

            var result = InputPreparation.Prepare(tested);

            CollectionAssert.AreEquivalent(expected, result);
        }

        [Test]
        public void PrepareStringWithWordFormingSymbolsReturnsListWithThatString()
        {
            string tested = @"QWERTYUIOPASDFGHJKLZXCVBNM1234567890qwertyui_-opasdfghjklzxcvbnm";

            var result = InputPreparation.Prepare(tested);

            Assert.That(result.Count == 1);
            CollectionAssert.Contains(result, tested);
        }

        [Test]
        public void PrepareUserStringReturnsListWithWords()
        {
            string tested = @"QWERTY qwe-rty _qwe-rty_ qwe-asd-zxc
                                _qwe-asd-zxc_ qwe_asd -qwe_asd- qwe_asd_qwe
                                -qwe_asd_qwe-";

            List<string> expected = new List<string>
            {
                "QWERTY",
                "qwe-rty",
                "_qwe-rty_",
                "qwe-asd-zxc",
                "_qwe-asd-zxc_",
                "qwe_asd",
                "-qwe_asd-",
                "qwe_asd_qwe",
                "-qwe_asd_qwe-"
            };

            var result = InputPreparation.Prepare(tested);

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PrepareNullStringThrowsArgumentNullException()
        {
            string tested = null;

            Assert.Throws(typeof(ArgumentNullException), () => InputPreparation.Prepare(tested));
        }

        [Test]
        public void PrepareWhiteSpacedStringReturnsEmptyList()
        {
            string tested = @"      ";

            var result = InputPreparation.Prepare(tested);

            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void PrepareEmptyStringReturnsEmptyList()
        {
            string tested = string.Empty;

            var result = InputPreparation.Prepare(tested);

            CollectionAssert.IsEmpty(result);
        }
    }
}
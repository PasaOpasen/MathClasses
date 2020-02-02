using System.Linq;
using Computator.NET.Core.Autocompletion;
using Computator.NET.Core.Autocompletion.DataSource;
using NUnit.Framework;

namespace Computator.NET.Core.IntegrationTests.Data
{
    [TestFixture]
    public class FunctionDetailsTests
    {
        [Test]
        public void TestLoading()
        {
            var array = (new FunctionsDetailsFileSource()).Details.ToArray();
            foreach (var keyValuePair in array)
            {
                Assert.IsNotNull(keyValuePair.Value);
                Assert.IsNotNull(keyValuePair.Value.Signature);
                Assert.IsNotNull(keyValuePair.Value.Type);
                Assert.IsNotNull(keyValuePair.Value.Category);
                Assert.IsNotNull(keyValuePair.Value.Description);
                Assert.IsNotNull(keyValuePair.Value.Title);
                Assert.IsNotNull(keyValuePair.Value.Url);

                if(!string.IsNullOrWhiteSpace(keyValuePair.Value.Signature))
                    Assert.AreEqual(keyValuePair.Key, keyValuePair.Value.Signature);
            }
        }
    }
}
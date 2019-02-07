namespace DiffLib_UnitTests
{
    using DiffLib.Enums;
    using DiffLib.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class EditScriptTests
    {
        [TestMethod]
        public void MatchOneLineTests()
        {
            bool ignoreCase = true;
            bool ignoreWhiteSpace = true;
            int leadingCharactersToIgnore = 0;
            bool supportChangeEditType = false;
            var txtDiff = new TextDiff(HashType.HashCode, ignoreCase, ignoreWhiteSpace, leadingCharactersToIgnore, supportChangeEditType);

            var editScript = txtDiff.Execute(new List<string>(), new List<string>());
            Assert.IsTrue(editScript.TotalEditLength == 0);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);

            editScript = null;
            editScript = txtDiff.Execute(new List<string>(), new List<string>() { "abc" });
            Assert.IsTrue(editScript.TotalEditLength == 1);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
            Assert.IsTrue(editScript[0].EditType == EditType.Insert);

            editScript = null;
            editScript = txtDiff.Execute(new List<string>() { "abc" }, new List<string>());
            Assert.IsTrue(editScript.TotalEditLength == 1);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
            Assert.IsTrue(editScript[0].EditType == EditType.Delete);

            editScript = null;
            editScript = txtDiff.Execute(new List<string>() { "abc" }, new List<string>() { "abc" });
            Assert.IsTrue(editScript.TotalEditLength == 0);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);

            editScript = null;
            editScript = txtDiff.Execute(new List<string>() { "acc" }, new List<string>() { "abc" });
            Assert.IsTrue(editScript.TotalEditLength == 2);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
            Assert.IsTrue(editScript[0].EditType == EditType.Delete);
            Assert.IsTrue(editScript[1].EditType == EditType.Insert);

            editScript = null;
            editScript = txtDiff.Execute(new List<string>() { "abc" }, new List<string>() { "abc" });
            Assert.IsTrue(editScript.TotalEditLength == 0);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
        }

        [TestMethod]
        public void MatchWhiteSpaceLineTests()
        {
            bool ignoreCase = true;
            bool ignoreWhiteSpace = true;
            int leadingCharactersToIgnore = 0;
            bool supportChangeEditType = false;
            var txtDiff = new TextDiff(HashType.HashCode, ignoreCase, ignoreWhiteSpace, leadingCharactersToIgnore, supportChangeEditType);

            var editScript = txtDiff.Execute(new List<string>() { "abc" }, new List<string>() { "  abc  " });
            Assert.IsTrue(editScript.TotalEditLength == 0);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
        }
    }
}

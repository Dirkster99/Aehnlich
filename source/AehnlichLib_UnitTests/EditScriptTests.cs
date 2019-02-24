namespace AehnlichLib_UnitTests
{
    using AehnlichLib.Enums;
    using AehnlichLib.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class EditScriptTests
    {
        [TestMethod]
        public void MatchTwoLineTests()
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
            editScript = txtDiff.Execute(new List<string>(), new List<string> { "abc", "ccc" });
            Assert.IsTrue(editScript.TotalEditLength == 2);
            Assert.IsTrue(editScript.Count != editScript.TotalEditLength);
            Assert.IsTrue(editScript.Count == 1);
            Assert.IsTrue(editScript[0].EditType == EditType.Insert);
            Assert.IsTrue(editScript[0].Length == 2);
            Assert.IsTrue(editScript[0].StartA == 0 && editScript[0].StartB == 0);

            editScript = null;
            editScript = txtDiff.Execute(new List<string> { "abc", "ccc" }, new List<string>() { });
            Assert.IsTrue(editScript.TotalEditLength == 2);
            Assert.IsTrue(editScript.Count != editScript.TotalEditLength);
            Assert.IsTrue(editScript.Count == 1);
            Assert.IsTrue(editScript[0].EditType == EditType.Delete);
            Assert.IsTrue(editScript[0].Length == 2);
            Assert.IsTrue(editScript[0].StartA == 0 && editScript[0].StartB == 0);

            editScript = null;
            editScript = txtDiff.Execute(new List<string> { "abc" }, new List<string> { "abc", "ccc" });
            Assert.IsTrue(editScript.TotalEditLength == 1);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
            Assert.IsTrue(editScript[0].EditType == EditType.Insert);
            Assert.IsTrue(editScript[0].Length == 1);
            Assert.IsTrue(editScript[0].StartA == 1 && editScript[0].StartB == 1);

            editScript = null;
            editScript = txtDiff.Execute(new List<string> { "abc", "ccc" }, new List<string> { "abc" });
            Assert.IsTrue(editScript.TotalEditLength == 1);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
            Assert.IsTrue(editScript[0].EditType == EditType.Delete);
            Assert.IsTrue(editScript[0].Length == 1);
            Assert.IsTrue(editScript[0].StartA == 1 && editScript[0].StartB == 1);

            // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            editScript = null;
            editScript = txtDiff.Execute(new List<string> { "abc", "ccc" },
                                         new List<string> { "abc", "ccc" });
            Assert.IsTrue(editScript.TotalEditLength == 0);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);

            editScript = null;
            editScript = txtDiff.Execute(new List<string> { "abc", "ccc" },
                                         new List<string> { "abc", "cxc" });
            Assert.IsTrue(editScript.TotalEditLength == 2);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
            Assert.IsTrue(editScript[0].EditType == EditType.Delete);
            Assert.IsTrue(editScript[0].Length == 1);
            Assert.IsTrue(editScript[0].StartA == 1 && editScript[0].StartB == 1);

            Assert.IsTrue(editScript[1].EditType == EditType.Insert);
            Assert.IsTrue(editScript[1].Length == 1);
            Assert.IsTrue(editScript[1].StartA == 1 && editScript[1].StartB == 1);

            editScript = null;
            editScript = txtDiff.Execute(new List<string> { "abc", "ccc" },
                                         new List<string> { "acc", "ccc" });
            Assert.IsTrue(editScript.TotalEditLength == 2);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
            Assert.IsTrue(editScript[0].EditType == EditType.Delete);
            Assert.IsTrue(editScript[0].Length == 1);
            Assert.IsTrue(editScript[0].StartA == 0 && editScript[0].StartB == 0);

            Assert.IsTrue(editScript[1].EditType == EditType.Insert);
            Assert.IsTrue(editScript[1].Length == 1);
            Assert.IsTrue(editScript[1].StartA == 0 && editScript[1].StartB == 0);

            editScript = null;
            editScript = txtDiff.Execute(new List<string> { "abc", "ccc" },
                                         new List<string> { "acc", "cxc" });
            Assert.IsTrue(editScript.TotalEditLength == 4);
            Assert.IsTrue(editScript.Count != editScript.TotalEditLength);
            Assert.IsTrue(editScript.Count == 2);
            Assert.IsTrue(editScript[0].EditType == EditType.Delete);
            Assert.IsTrue(editScript[0].Length == 2);
            Assert.IsTrue(editScript[0].StartA == 0 && editScript[0].StartB == 0);

            Assert.IsTrue(editScript[1].EditType == EditType.Insert);
            Assert.IsTrue(editScript[1].Length == 2);
            Assert.IsTrue(editScript[1].StartA == 0 && editScript[1].StartB == 0);
        }

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
            editScript = txtDiff.Execute(new List<string>(), new List<string> { "abc" });
            Assert.IsTrue(editScript.TotalEditLength == 1);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
            Assert.IsTrue(editScript[0].EditType == EditType.Insert);
            Assert.IsTrue(editScript[0].Length == 1);

            editScript = null;
            editScript = txtDiff.Execute(new List<string> { "abc" }, new List<string>());
            Assert.IsTrue(editScript.TotalEditLength == 1);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
            Assert.IsTrue(editScript[0].EditType == EditType.Delete);
            Assert.IsTrue(editScript[0].Length == 1);

            editScript = null;
            editScript = txtDiff.Execute(new List<string> { "abc" }, new List<string> { "abc" });
            Assert.IsTrue(editScript.TotalEditLength == 0);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);

            editScript = null;
            editScript = txtDiff.Execute(new List<string> { "acc" }, new List<string> { "abc" });
            Assert.IsTrue(editScript.TotalEditLength == 2);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
            Assert.IsTrue(editScript[0].EditType == EditType.Delete);
            Assert.IsTrue(editScript[1].EditType == EditType.Insert);
            Assert.IsTrue(editScript[0].Length == 1);
        }

        [TestMethod]
        public void ChangeEditTypeTests()
        {
            bool ignoreCase = true;
            bool ignoreWhiteSpace = true;
            int leadingCharactersToIgnore = 0;
            bool supportChangeEditType = true;

            var txtDiff = new TextDiff(HashType.HashCode, ignoreCase, ignoreWhiteSpace, leadingCharactersToIgnore, supportChangeEditType);
            var editScript = txtDiff.Execute(new List<string> { "acc" }, new List<string> { "abc" });
            Assert.IsTrue(editScript.TotalEditLength == 2);
            Assert.IsTrue(editScript.Count != editScript.TotalEditLength);
            Assert.IsTrue(editScript.Count == 1);
            Assert.IsTrue(editScript[0].EditType == EditType.Change);
            Assert.IsTrue(editScript[0].Length == 1);

            editScript = null;
            editScript = txtDiff.Execute(new List<string> { "abc" }, new List<string> { "abc" });
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

            var editScript = txtDiff.Execute(new List<string> { "abc" }, new List<string> { "  abc  " });
            Assert.IsTrue(editScript.TotalEditLength == 0);
            Assert.IsTrue(editScript.Count == editScript.TotalEditLength);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace GMS.Framework.Utility
{
    /// <summary>
    /// Diff两个字符串
    /// </summary>
    public class Diff
    {
        /// <summary>
        /// <para>toWord为真时，适合英文，将以单词为一个单位做比较，为false，将每个字符做比较，适合无html代码的纯中文</para>
        /// <para>请在Web显示输出时加入2个样式，参考：http://www.rohland.co.za/index.php/2009/10/31/csharp-html-diff-algorithm/ </para>
        /// </summary>
        /// <param name="oldText"></param>
        /// <param name="newText"></param>
        /// <param name="toWord"></param>
        /// <returns></returns>
        public static string Build(string oldText, string newText, bool toWord = false)
        {
            HtmlDiff diffHelper = new HtmlDiff(oldText ?? string.Empty, newText ?? string.Empty, toWord);
            return diffHelper.Build();
        }
    }
    
    /// <summary>
    /// Diff两个字符串
    /// </summary>
    internal class HtmlDiff
    {

        private StringBuilder content;
        private string oldText, newText;
        private bool toWord;
        private string[] oldWords, newWords;
        Dictionary<string, List<int>> wordIndices;
        private string[] specialCaseOpeningTags = new string[] { "<strong[\\>\\s]+", "<b[\\>\\s]+", "<i[\\>\\s]+", "<big[\\>\\s]+", "<small[\\>\\s]+", "<u[\\>\\s]+", "<sub[\\>\\s]+", "<sup[\\>\\s]+", "<strike[\\>\\s]+", "<s[\\>\\s]+" };
        private string[] specialCaseClosingTags = new string[] { "</strong>", "</b>", "</i>", "</big>", "</small>", "</u>", "</sub>", "</sup>", "</strike>", "</s>" };

        
        /// <summary>
        /// Initializes a new instance of the <see cref="Diff"/> class.
        /// </summary>
        /// <param name="oldText">The old text.</param>
        /// <param name="newText">The new text.</param>
        public HtmlDiff(string oldText, string newText, bool toWord = true)
        {
            this.oldText = oldText;
            this.newText = newText;
            this.toWord = toWord;

            this.content = new StringBuilder();
        }

        /// <summary>
        /// Builds the HTML diff output
        /// </summary>
        /// <returns>HTML diff markup</returns>
        public string Build()
        {
            this.SplitInputsToWords();

            this.IndexNewWords();

            var operations = this.Operations();

            foreach (var item in operations)
            {
                this.PerformOperation(item);
            }

            return this.content.ToString();
        }

        private void IndexNewWords()
        {           
            this.wordIndices = new Dictionary<string, List<int>>();
            for (int i = 0; i < this.newWords.Length; i++)
            {
                string word = this.newWords[i];

                if (this.wordIndices.ContainsKey(word))
                {
                    this.wordIndices[word].Add(i);
                }
                else
                {
                    this.wordIndices[word] = new List<int>();
                    this.wordIndices[word].Add(i);
                }
            }
        }

        private void SplitInputsToWords()
        {
            if (this.toWord)
            {
                this.oldWords = ConvertHtmlToListOfWords(this.Explode(this.oldText));
                this.newWords = ConvertHtmlToListOfWords(this.Explode(this.newText));
            }
            else
            {
                this.oldWords = this.Explode(this.oldText);
                this.newWords = this.Explode(this.newText);
            }
        }

        private string[] ConvertHtmlToListOfWords(string[] characterString)
        {
            DiffMode mode = DiffMode.character;
            string current_word = String.Empty;
            List<string> words = new List<string>();

            foreach (var character in characterString)
            {
                switch (mode)
                {
                    case DiffMode.character:

                        if (this.IsStartOfTag(character))
                        {
                            if (current_word != String.Empty)
                            {
                                words.Add(current_word);
                            }

                            current_word = "<";
                            mode = DiffMode.tag;
                        }
                        else if (Regex.IsMatch(character, "\\s"))
                        {
                            if (current_word != String.Empty)
                            {
                                words.Add(current_word);
                            }
                            current_word = character;
                            mode = DiffMode.whitespace;
                        }
                        else
                        {
                            current_word += character;
                        }

                        break;
                    case DiffMode.tag:

                        if (this.IsEndOfTag(character))
                        {
                            current_word += ">";
                            words.Add(current_word);
                            current_word = "";

                            if (IsWhiteSpace(character))
                            {
                                mode = DiffMode.whitespace;
                            }
                            else
                            {
                                mode = DiffMode.character;
                            }
                        }
                        else
                        {
                            current_word += character;
                        }

                        break;
                    case DiffMode.whitespace:

                        if (this.IsStartOfTag(character))
                        {
                            if (current_word != String.Empty)
                            {
                                words.Add(current_word);
                            }
                            current_word = "<";
                            mode = DiffMode.tag;
                        }
                        else if (Regex.IsMatch(character, "\\s"))
                        {
                            current_word += character;
                        }
                        else
                        {
                            if (current_word != String.Empty)
                            {
                                words.Add(current_word);
                            }

                            current_word = character;
                            mode = DiffMode.character;
                        }

                        break;
                    default:
                        break;
                }


            }
            if (current_word != string.Empty)
            {
                words.Add(current_word);
            }

            return words.ToArray();
        }

        private bool IsStartOfTag(string val)
        {
            return val == "<";
        }

        private bool IsEndOfTag(string val)
        {
            return val == ">";
        }

        private bool IsWhiteSpace(string value)
        {
            return Regex.IsMatch(value, "\\s");
        }

        private string[] Explode(string value)
        {
            return Regex.Split(value, "");
        }

        private void PerformOperation(DiffOperation operation)
        {
            switch (operation.Action)
            {
                case DiffAction.equal:
                    this.ProcessEqualOperation(operation);
                    break;
                case DiffAction.delete:
                    this.ProcessDeleteOperation(operation, "diffdel");
                    break;
                case DiffAction.insert:
                    this.ProcessInsertOperation(operation, "diffins");
                    break;
                case DiffAction.none:
                    break;
                case DiffAction.replace:
                    this.ProcessReplaceOperation(operation);
                    break;
                default:
                    break;
            }
        }

        private void ProcessReplaceOperation(DiffOperation operation)
        {
            this.ProcessDeleteOperation(operation, "diffmod");
            this.ProcessInsertOperation(operation, "diffmod");
        }

        private void ProcessInsertOperation(DiffOperation operation, string cssClass)
        {
            this.InsertTag("ins", cssClass, this.newWords.Where((s, pos) => pos >= operation.StartInNew && pos < operation.EndInNew).ToList());
        }

        private void ProcessDeleteOperation(DiffOperation operation, string cssClass)
        {
            var text = this.oldWords.Where((s, pos) => pos >= operation.StartInOld && pos < operation.EndInOld).ToList();
            this.InsertTag("del", cssClass, text);
        }

        private void ProcessEqualOperation(DiffOperation operation)
        {
            var result = this.newWords.Where((s, pos) => pos >= operation.StartInNew && pos < operation.EndInNew).ToArray();
            this.content.Append(String.Join("", result));
        }


        /// <summary>
        /// This method encloses words within a specified tag (ins or del), and adds this into "content", 
        /// with a twist: if there are words contain tags, it actually creates multiple ins or del, 
        /// so that they don't include any ins or del. This handles cases like
        /// old: '<p>a</p>'
        /// new: '<p>ab</p><p>c</b>'
        /// diff result: '<p>a<ins>b</ins></p><p><ins>c</ins></p>'
        /// this still doesn't guarantee valid HTML (hint: think about diffing a text containing ins or
        /// del tags), but handles correctly more cases than the earlier version.
        /// 
        /// P.S.: Spare a thought for people who write HTML browsers. They live in this ... every day.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="cssClass"></param>
        /// <param name="words"></param>
        private void InsertTag(string tag, string cssClass, List<string> words)
        {
            while (true)
            {
                if (words.Count == 0)
                {
                    break;
                }

                var nonTags = ExtractConsecutiveWords(words, x => !this.IsTag(x));

                string specialCaseTagInjection = string.Empty;
                bool specialCaseTagInjectionIsBefore = false;

                if (nonTags.Length != 0)
                {
                    string text = this.WrapText(string.Join("", nonTags), tag, cssClass);

                    this.content.Append(text);
                }
                else
                {
                    // Check if strong tag

                    if (this.specialCaseOpeningTags.FirstOrDefault(x => Regex.IsMatch(words[0], x)) != null)
                    {
                        specialCaseTagInjection = "<ins class='mod'>";
                        if (tag == "del")
                        {
                            words.RemoveAt(0);
                        }
                    }
                    else if (this.specialCaseClosingTags.Contains(words[0]))
                    {
                        specialCaseTagInjection = "</ins>";
                        specialCaseTagInjectionIsBefore = true;
                        if (tag == "del")
                        {
                            words.RemoveAt(0);
                        }
                    }

                }

                if (words.Count == 0 && specialCaseTagInjection.Length == 0)
                {
                    break;
                }

                if (specialCaseTagInjectionIsBefore)
                {
                    this.content.Append(specialCaseTagInjection + String.Join("", this.ExtractConsecutiveWords(words, x => this.IsTag(x))));
                }
                else
                {
                    this.content.Append(String.Join("", this.ExtractConsecutiveWords(words, x => this.IsTag(x))) + specialCaseTagInjection);
                }
            }
        }

        private string WrapText(string text, string tagName, string cssClass)
        {
            return string.Format("<{0} class='{1}'>{2}</{0}>", tagName, cssClass, text);
        }

        private string[] ExtractConsecutiveWords(List<string> words, Func<string, bool> condition)
        {
            int? indexOfFirstTag = null;

            for (int i = 0; i < words.Count; i++)
            {
                string word = words[i];

                if (!condition(word))
                {
                    indexOfFirstTag = i;
                    break;
                }
            }

            if (indexOfFirstTag != null)
            {
                var items = words.Where((s, pos) => pos >= 0 && pos < indexOfFirstTag).ToArray();
                if (indexOfFirstTag.Value > 0)
                {
                    words.RemoveRange(0, indexOfFirstTag.Value);
                }
                return items;
            }
            else
            {
                var items = words.Where((s, pos) => pos >= 0 && pos <= words.Count).ToArray();
                words.RemoveRange(0, words.Count);
                return items;
            }
        }

        private bool IsTag(string item)
        {            
            bool isTag = IsOpeningTag(item) || IsClosingTag(item);
            return isTag;
        }

        private bool IsOpeningTag(string item)
        {
            return Regex.IsMatch(item, "^\\s*<[^>]+>\\s*$");
        }

        private bool IsClosingTag(string item)
        {
            return Regex.IsMatch(item, "^\\s*</[^>]+>\\s*$");
        }


        private List<DiffOperation> Operations()
        {
            int positionInOld = 0, positionInNew = 0;
            List<DiffOperation> operations = new List<DiffOperation>();

            var matches = this.MatchingBlocks();

            matches.Add(new DiffMatch(this.oldWords.Length, this.newWords.Length, 0));

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];

                bool matchStartsAtCurrentPositionInOld = (positionInOld == match.StartInOld);
                bool matchStartsAtCurrentPositionInNew = (positionInNew == match.StartInNew);

                DiffAction action = DiffAction.none;

                if (matchStartsAtCurrentPositionInOld == false
                    && matchStartsAtCurrentPositionInNew == false)
                {
                    action = DiffAction.replace;
                }
                else if (matchStartsAtCurrentPositionInOld == true
                    && matchStartsAtCurrentPositionInNew == false)
                {
                    action = DiffAction.insert;
                }
                else if (matchStartsAtCurrentPositionInOld == false
                    && matchStartsAtCurrentPositionInNew == true)
                {
                    action = DiffAction.delete;
                }
                else // This occurs if the first few words are the same in both versions
                {
                    action = DiffAction.none;
                }

                if (action != DiffAction.none)
                {
                    operations.Add(
                        new DiffOperation(action,
                            positionInOld,
                            match.StartInOld,
                            positionInNew,
                            match.StartInNew));
                }

                if (match.Size != 0)
                {
                    operations.Add(new DiffOperation(
                        DiffAction.equal,
                        match.StartInOld,
                        match.EndInOld,
                        match.StartInNew,
                        match.EndInNew));

                }

                positionInOld = match.EndInOld;
                positionInNew = match.EndInNew;
            }

            return operations;

        }

        private List<DiffMatch> MatchingBlocks()
        {
            List<DiffMatch> matchingBlocks = new List<DiffMatch>();
            this.FindMatchingBlocks(0, this.oldWords.Length, 0, this.newWords.Length, matchingBlocks);
            return matchingBlocks;
        }


        private void FindMatchingBlocks(int startInOld, int endInOld, int startInNew, int endInNew, List<DiffMatch> matchingBlocks)
        {
            var match = this.FindMatch(startInOld, endInOld, startInNew, endInNew);

            if (match != null)
            {
                if (startInOld < match.StartInOld && startInNew < match.StartInNew)
                {
                    this.FindMatchingBlocks(startInOld, match.StartInOld, startInNew, match.StartInNew, matchingBlocks);
                }

                matchingBlocks.Add(match);

                if (match.EndInOld < endInOld && match.EndInNew < endInNew)
                {
                    this.FindMatchingBlocks(match.EndInOld, endInOld, match.EndInNew, endInNew, matchingBlocks);
                }

            }
        }


        private DiffMatch FindMatch(int startInOld, int endInOld, int startInNew, int endInNew)
        {
            int bestMatchInOld = startInOld;
            int bestMatchInNew = startInNew;
            int bestMatchSize = 0;

            Dictionary<int, int> matchLengthAt = new Dictionary<int, int>();

            for (int indexInOld = startInOld; indexInOld < endInOld; indexInOld++)
            {
                var newMatchLengthAt = new Dictionary<int, int>();

                string index = this.oldWords[indexInOld];

                if (!this.wordIndices.ContainsKey(index))
                {
                    matchLengthAt = newMatchLengthAt;
                    continue;
                }

                foreach (var indexInNew in this.wordIndices[index])
                {
                    if (indexInNew < startInNew)
                    {
                        continue;
                    }

                    if (indexInNew >= endInNew)
                    {
                        break;
                    }


                    int newMatchLength = (matchLengthAt.ContainsKey(indexInNew - 1) ? matchLengthAt[indexInNew - 1] : 0) + 1;
                    newMatchLengthAt[indexInNew] = newMatchLength;

                    if (newMatchLength > bestMatchSize)
                    {
                        bestMatchInOld = indexInOld - newMatchLength + 1;
                        bestMatchInNew = indexInNew - newMatchLength + 1;
                        bestMatchSize = newMatchLength;
                    }
                }

                matchLengthAt = newMatchLengthAt;
            }

            return bestMatchSize != 0 ? new DiffMatch(bestMatchInOld, bestMatchInNew, bestMatchSize) : null;
        }

    }

    public class DiffMatch
    {
        public DiffMatch(int startInOld, int startInNew, int size)
        {
            this.StartInOld = startInOld;
            this.StartInNew = startInNew;
            this.Size = size;
        }

        public int StartInOld { get; set; }
        public int StartInNew { get; set; }
        public int Size { get; set; }

        public int EndInOld
        {
            get
            {
                return this.StartInOld + this.Size;
            }
        }

        public int EndInNew
        {
            get
            {
                return this.StartInNew + this.Size;
            }
        }

    }

    public class DiffOperation
    {
        public DiffAction Action { get; set; }
        public int StartInOld { get; set; }
        public int EndInOld { get; set; }
        public int StartInNew { get; set; }
        public int EndInNew { get; set; }

        public DiffOperation(DiffAction action, int startInOld, int endInOld, int startInNew, int endInNew)
        {
            this.Action = action;
            this.StartInOld = startInOld;
            this.EndInOld = endInOld;
            this.StartInNew = startInNew;
            this.EndInNew = endInNew;
        }
    }

    public enum DiffMode
    {
        character,
        tag,
        whitespace,
    }

    public enum DiffAction
    {
        equal,
        delete,
        insert,
        none,
        replace
    }
}

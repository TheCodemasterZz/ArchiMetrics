// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiLineCommentLanguageRule.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MultiLineCommentLanguageRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Trivia
{
	using ArchiMetrics.Common;
	using Roslyn.Compilers.CSharp;

	internal class MultiLineCommentLanguageRule : CommentLanguageRuleBase
	{
		public MultiLineCommentLanguageRule(ISpellChecker spellChecker, IKnownWordList knownWordList)
			: base(spellChecker, knownWordList)
		{
		}

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MultiLineCommentTrivia; }
		}
	}
}

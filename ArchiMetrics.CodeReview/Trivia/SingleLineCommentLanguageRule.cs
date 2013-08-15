// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleLineCommentLanguageRule.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SingleLineCommentLanguageRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Trivia
{
	using ArchiMetrics.Common;
	using Roslyn.Compilers.CSharp;

	internal class SingleLineCommentLanguageRule : CommentLanguageRuleBase
	{
		public SingleLineCommentLanguageRule(ISpellChecker spellChecker, IKnownWordList knownWordList)
			: base(spellChecker, knownWordList)
		{
		}

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.SingleLineCommentTrivia; }
		}
	}
}

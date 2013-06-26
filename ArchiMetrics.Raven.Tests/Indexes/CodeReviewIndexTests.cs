// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeReviewIndexTests.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeReviewIndexTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Raven.Tests.Indexes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Common.Documents;
	using NUnit.Framework;
	using Raven.Indexes;

	public class CodeReviewIndexTests : IndexTestBase<ErrorData, ErrorData, CodeReviewIndexTests.FakeCodeReviewIndex>
	{
		[Test]
		public void WhenReducingThenCreatesGroupedData()
		{
			var data = new[]
				             {
					             new ErrorData
						             {
							             DistinctLoc = 1, 
										 Effort = 2, 
										 Error = "Something", 
										 Occurrences = 1, 
										 ProjectName = "Project", 
										 ProjectVersion = "1"
						             }, 
									 new ErrorData
						             {
							             DistinctLoc = 1, 
										 Effort = 2, 
										 Error = "Something", 
										 Occurrences = 1, 
										 ProjectName = "Project", 
										 ProjectVersion = "1"
						             }
				             };

			var result = PerformMapReduce(data);

			Assert.AreEqual(1, result.Length);
		}

		public class FakeCodeReviewIndex : CodeReviewIndex, ITestIndex<ErrorData, ErrorData>
		{
			public Expression<Func<IEnumerable<ErrorData>, IEnumerable>> GetMap()
			{
				return Map;
			}

			public Expression<Func<IEnumerable<ErrorData>, IEnumerable>> GetReduce()
			{
				return Reduce;
			}
		}
	}
}
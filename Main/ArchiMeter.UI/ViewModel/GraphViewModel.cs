// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphViewModel.cs" company="Roche">
//   Copyright � Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GraphViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;

	using ArchiMeter.Analysis;
	using ArchiMeter.Common;

	internal class GraphViewModel : ViewModelBase
	{
		private readonly DependencyAnalyzer _analyzer = new DependencyAnalyzer();
		private readonly IEdgeTransformer _filter;
		private readonly IEdgeItemsRepository _repository;
		private EdgeItem[] _allEdges;
		private ProjectGraph _graphToVisualize;

		public GraphViewModel(IEdgeItemsRepository repository, IEdgeTransformer filter, ISolutionEdgeItemsRepositoryConfig config)
			: base(config)
		{
			_repository = repository;
			_filter = filter;
			LoadAllEdges();
		}

		public ProjectGraph GraphToVisualize
		{
			get
			{
				return _graphToVisualize;
			}

			private set
			{
				if (_graphToVisualize != value)
				{
					_graphToVisualize = value;
					this.RaisePropertyChanged();
				}
			}
		}

		protected override void Update(bool forceUpdate)
		{
			if (forceUpdate)
			{
				this.LoadAllEdges();
			}
			else
			{
				this.UpdateInternal();
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			_graphToVisualize = null;
			_allEdges = null;
			base.Dispose(isDisposing);
		}

		private async void UpdateInternal()
		{
			this.IsLoading = true;

			var nonEmptySourceItems = (await _filter.TransformAsync(_allEdges))
				.ToArray();

			var circularReferences = (await _analyzer.GetCircularReferences(nonEmptySourceItems))
				.ToArray();

			var projectVertices = nonEmptySourceItems
				.AsParallel()
				.SelectMany(item =>
					{
						var isCircular = circularReferences.Any(c => c.Contains(item));
						return this.CreateVertices(item, isCircular);
					})
				.GroupBy(v => v.Name)
				.Select(grouping => grouping.First())
				.ToArray();

			var edges =
				nonEmptySourceItems
				.Where(e => !string.IsNullOrWhiteSpace(e.Dependency))
				.Select(
					dependencyItemViewModel =>
					new ProjectEdge(
						projectVertices.First(item => item.Name == dependencyItemViewModel.Dependant),
						projectVertices.First(item => item.Name == dependencyItemViewModel.Dependency)))
								   .Where(e => e.Target.Name != e.Source.Name);
			var g = new ProjectGraph();

			foreach (var vertex in projectVertices)
			{
				g.AddVertex(vertex);
			}

			foreach (var edge in edges)
			{
				g.AddEdge(edge);
			}

			this.GraphToVisualize = g;
			this.IsLoading = false;
		}

		private IEnumerable<Vertex> CreateVertices(EdgeItem item, bool isCircular)
		{
			yield return new Vertex(item.Dependant, isCircular, item.DependantComplexity, item.DependantMaintainabilityIndex, item.DependantLinesOfCode);
			if (!string.IsNullOrWhiteSpace(item.Dependency))
			{
				yield return
					new Vertex(item.Dependency, isCircular, item.DependencyComplexity, item.DependencyMaintainabilityIndex, item.DependencyLinesOfCode, item.CodeIssues);
			}
		}

		private void LoadAllEdges()
		{
			IsLoading = true;
			_repository.GetEdgesAsync()
				.ContinueWith(t =>
				{
					_allEdges = t.Result.Where(e => e.Dependant != e.Dependency).ToArray();
					UpdateInternal();
				});
		}
	}
}
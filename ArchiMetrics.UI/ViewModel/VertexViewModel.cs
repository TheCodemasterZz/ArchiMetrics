// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VertexViewModel.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the VertexViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Threading;
	using System.Windows.Data;
	using System.Windows.Input;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Structure;
	using ArchiMetrics.UI.Support;

	internal class VertexViewModel : VertexViewModelBase
	{
		private readonly IAsyncFactory<IEnumerable<IModelNode>, IEnumerable<ModelEdgeItem>> _edgeFactory;
		private readonly IProvider<string, ObservableCollection<TransformRule>> _transformRulesProvider;
		private readonly object _syncLock = new object();
		private readonly DelegateCommand _updateCommand;
		private readonly IAppContext _config;
		private ObservableCollection<ModelEdgeItem> _edgeItems;

		public VertexViewModel(
			IVertexRepository repository,
			IAsyncFactory<IEnumerable<IModelNode>, IEnumerable<ModelEdgeItem>> edgeFactory,
			IProvider<string, ObservableCollection<TransformRule>> transformRulesProvider,
			ISyntaxTransformer filter,
			IAppContext config)
			: base(repository, filter, config)
		{
			_edgeFactory = edgeFactory;
			_transformRulesProvider = transformRulesProvider;
			_config = config;
			EdgeItems = new ObservableCollection<ModelEdgeItem>();
			UpdateImpl(true);
			_updateCommand = new DelegateCommand(o => true, o => UpdateImpl(false));
		}

		public ObservableCollection<ModelEdgeItem> EdgeItems
		{
			get
			{
				return _edgeItems;
			}

			private set
			{
				if (value != _edgeItems)
				{
					_edgeItems = value;
					RaisePropertyChanged();
				}
			}
		}

		public ICommand UpdateList
		{
			get
			{
				return _updateCommand;
			}
		}

		protected async override void UpdateInternal(CancellationToken cancellationToken)
		{
			IsLoading = true;
			var rules = _transformRulesProvider.Get(_config.RulesSource);
			var results = await Filter.Transform(AllVertices, rules, cancellationToken);
			var edges = await _edgeFactory.Create(results.OfType<IModelNode>(), cancellationToken);
			var newCollection = new ObservableCollection<ModelEdgeItem>(edges.Distinct());
			VertexTransforms = rules;
			EdgeItems = newCollection;
			IsLoading = false;
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_edgeItems.Clear();
			}

			base.Dispose(isDisposing);
		}
	}
}
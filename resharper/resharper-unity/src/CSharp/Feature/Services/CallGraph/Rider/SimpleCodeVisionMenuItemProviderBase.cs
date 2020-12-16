using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Resources;
using JetBrains.ReSharper.Plugins.Unity.CSharp.Daemon.CallGraph;
using JetBrains.ReSharper.Plugins.Unity.CSharp.Daemon.Stages.CallGraphStage;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Unity.CSharp.Feature.Services.CallGraph.Rider
{
    public abstract class SimpleCodeVisionMenuItemProviderBase : ICallGraphCodeVisionMenuItemProvider
    {
        private readonly ISolution mySolution;

        protected SimpleCodeVisionMenuItemProviderBase(ISolution solution)
        {
            mySolution = solution;
        }

        public IEnumerable<BulbMenuItem> GetMenuItems(IMethodDeclaration methodDeclaration, ITextControl textControl, DaemonProcessKind processKind)
        {
            methodDeclaration.GetPsiServices().Locks.AssertReadAccessAllowed();
            if (!CheckCallGraph(methodDeclaration, processKind))
                return EmptyList<BulbMenuItem>.Enumerable;
            
            var bulbActions = GetActions(methodDeclaration);
            var result = new LocalList<BulbMenuItem>();
            
            foreach (var bulb in bulbActions)
            {
                var item = UnityCallGraphUtil.BulbActionToMenuItem(bulb, textControl, mySolution, BulbThemedIcons.ContextAction.Id);
                
                result.Add(item);
            }

            return result.ResultingList();
        }

        protected virtual bool CheckCallGraph([NotNull] IMethodDeclaration methodDeclaration, DaemonProcessKind processKind) => true;

        protected abstract IEnumerable<IBulbAction> GetActions([NotNull] IMethodDeclaration methodDeclaration);
    }
}
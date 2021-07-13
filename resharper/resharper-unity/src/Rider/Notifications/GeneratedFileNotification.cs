using System;
using JetBrains.Annotations;
using JetBrains.Collections.Viewable;
using JetBrains.Core;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Features.Inspections.Bookmarks.NumberedBookmarks;
using JetBrains.ReSharper.Plugins.Unity.AsmDefNew.Psi.Caches;
using JetBrains.ReSharper.Plugins.Unity.ProjectModel;
using JetBrains.ReSharper.Plugins.Unity.Rider.Protocol;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Backend.Features.Notifications;
using JetBrains.Rider.Backend.Features.ProjectModel;
using JetBrains.Rider.Backend.Features.TextControls;
using JetBrains.Util;
using JetBrains.Util.Extension;

namespace JetBrains.ReSharper.Plugins.Unity.Rider.Notifications
{
    [SolutionComponent]
    public class GeneratedFileNotification
    {
        public GeneratedFileNotification(Lifetime lifetime,
                                         FrontendBackendHost frontendBackendHost,
                                         BackendUnityHost backendUnityHost,
                                         UnitySolutionTracker solutionTracker,
                                         ISolution solution,
                                         AsmDefNameCache asmDefNameCache,
                                         [CanBeNull] RiderTextControlHost textControlHost = null,
                                         [CanBeNull] SolutionLifecycleHost solutionLifecycleHost = null,
                                         [CanBeNull] NotificationPanelHost notificationPanelHost = null)
        {
            // TODO: Why are these [CanBeNull]?
            if (solutionLifecycleHost == null || textControlHost == null || notificationPanelHost == null)
                return;

            if (!solutionTracker.IsUnityGeneratedProject.Value)
                return;

            var fullStartupFinishedLifetimeDefinition = new LifetimeDefinition(lifetime);
            solutionLifecycleHost.FullStartupFinished.Advise(fullStartupFinishedLifetimeDefinition.Lifetime, _ =>
            {
                textControlHost.ViewHostTextControls(lifetime, (lt, id, host) =>
                {
                    var projectFile = host.ToProjectFile(solution);
                    if (projectFile == null)
                        return;

                    if (!projectFile.Location.ExtensionNoDot.Equals("csproj", StringComparison.OrdinalIgnoreCase))
                        return;

                    backendUnityHost.BackendUnityModel.ViewNotNull(lt, (modelLifetime, backendUnityModel) =>
                    {
                        var name = projectFile.Location.NameWithoutExtension;

                        IPath path;
                        using (ReadLockCookie.Create())
                        {
                            path = asmDefNameCache.GetPathFor(name)?.TryMakeRelativeTo(solution.SolutionFilePath);
                        }

                        var elements = new LocalList<INotificationPanelHyperlink>();
                        if (path != null)
                        {
                            var strPath = path.Components.Join("/").RemoveStart("../");
                            elements.Add(new NotificationPanelCallbackHyperlink(modelLifetime,
                                "Edit corresponding .asmdef in Unity", false,
                                () =>
                                {
                                    frontendBackendHost.Do(t =>
                                    {
                                        t.AllowSetForegroundWindow.Start(modelLifetime, Unit.Instance)
                                            .Result.AdviseOnce(modelLifetime, __ =>
                                            {
                                                backendUnityHost.BackendUnityModel.Value?.ShowFileInUnity.Fire(strPath);
                                            });
                                    });
                                }));
                        }

                        notificationPanelHost.AddNotificationPanel(modelLifetime, host,
                            new NotificationPanel("This file is generated by Unity. Any changes made will be lost.",
                                "UnityGeneratedFile", elements.ToArray()));
                    });
                });

                fullStartupFinishedLifetimeDefinition.Terminate();
            });
        }
    }
}
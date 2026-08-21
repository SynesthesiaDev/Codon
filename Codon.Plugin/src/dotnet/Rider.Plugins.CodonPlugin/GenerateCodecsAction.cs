// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using JetBrains.Application.DataContext;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.Actions;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.Util;

namespace Rider.Plugins.CodonPlugin;

[ContextAction(
    Name = "GenerateCodecs",
    Description = "Generates codec fields for the class",
    GroupType = typeof(CSharpContextActions),
    Disabled = false,
    Priority = 1)]
public class GenerateCodecsAction(ICSharpContextActionDataProvider provider) : ContextActionBase
{
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
        var classDecl = GetClassDeclaration(context);
        var enabled = classDecl?.PrimaryConstructorDeclaration != null;
        presentation.Visible = classDecl != null;
        return enabled;
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        var classDecl = provider.GetSelectedElement<IClassLikeDeclaration>();
        if (classDecl?.PrimaryConstructorDeclaration == null) return null;

        var className = classDecl.DeclaredElement?.ShortName;
        if (className == null) return null;

        var members = ExtractMembers(classDecl.PrimaryConstructorDeclaration);
        if (members.Count == 0) return null;

        var codecField = CodedBuilder.BuildCodecField(className, binary: false, members);
        var binaryCodecField = CodedBuilder.BuildCodecField(className, binary: true, members);

        var factory = CSharpElementFactory.GetInstance(classDecl);

        var codecMember = (IClassMemberDeclaration)factory.CreateTypeMemberDeclaration(codecField);
        var binaryCodecMember = (IClassMemberDeclaration)factory.CreateTypeMemberDeclaration(binaryCodecField);

        classDecl.AddClassMemberDeclaration(codecMember);
        classDecl.AddClassMemberDeclaration(binaryCodecMember);

        return null;
    }

    public override string Text => "Generate Codec";

    public static IClassLikeDeclaration GetClassDeclaration(IDataContext context)
    {
        var solution = context.GetData(ProjectModelDataConstants.SOLUTION);
        var textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL);
        if (solution == null || textControl == null) return null;

        return TextControlToPsi.GetElement<IClassLikeDeclaration>(solution, textControl);
    }

    public static IReadOnlyList<(string Name, string Type, bool Nullable, bool IsEnum)> ExtractMembers(
        IPrimaryConstructorDeclaration primaryCtor)
    {
        var result = new List<(string, string, bool, bool)>();

        foreach (var p in primaryCtor.Params.ParameterDeclarations)
        {
            var name = p.NameIdentifier!.Name;
            var isEnum = p.Type.IsEnumType();
            var isNullable = p.Type.IsNullable();
            var typeText = p.Type.GetPresentableName(CSharpLanguage.Instance!);
            result.Add((name, typeText, isNullable, isEnum));
        }

        return result;
    }

    public override bool IsAvailable(IUserDataHolder cache)
    {
        var classDecl = provider.GetSelectedElement<IClassLikeDeclaration>();
        return classDecl?.PrimaryConstructorDeclaration != null;
    }
}

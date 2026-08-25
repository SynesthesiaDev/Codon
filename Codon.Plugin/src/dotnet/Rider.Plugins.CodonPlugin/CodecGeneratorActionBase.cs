// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.Util;

namespace Rider.Plugins.CodonPlugin;

public record CodecGeneratorActionBase(ICSharpContextActionDataProvider Provider, CodecGeneratorActionBase.CodecKind Kind)
{
    public enum CodecKind
    {
        Struct,
        Binary
    }

    public Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        var classDecl = Provider.GetSelectedElement<IClassLikeDeclaration>();
        if (classDecl?.PrimaryConstructorDeclaration == null) return null;

        var className = classDecl.DeclaredElement?.ShortName;
        if (className == null) return null;

        var members = ExtractMembers(classDecl.PrimaryConstructorDeclaration);
        if (members.Count == 0) return null;

        var field = CodedBuilder.BuildCodecField(className, binary: Kind == CodecKind.Binary, members);

        var factory = CSharpElementFactory.GetInstance(classDecl);
        var member = (IClassMemberDeclaration)factory.CreateTypeMemberDeclaration(field);

        classDecl.AddClassMemberDeclaration(member);

        return null;
    }

    public static IReadOnlyList<(string Name, string Type, bool Nullable, bool IsEnum)> ExtractMembers(
        IPrimaryConstructorDeclaration primaryCtor)
    {
        var result = new List<(string, string, bool, bool)>();

        foreach (var p in primaryCtor.Params.ParameterDeclarations)
        {
            var name = p.NameIdentifier!.Name;

            var underlyingType = p.Type.GetNullableUnderlyingType() ?? p.Type;
            var isEnum = underlyingType.IsEnumType();

            var typeText = p.Type.GetPresentableName(CSharpLanguage.Instance!);

            var isNullable = p.Type.IsNullable() || typeText.EndsWith("?");

            result.Add((name, typeText, isNullable, isEnum));
        }

        return result;
    }

    public bool IsAvailable(IUserDataHolder cache)
    {
        var classDecl = Provider.GetSelectedElement<IClassLikeDeclaration>();
        return classDecl?.PrimaryConstructorDeclaration != null;
    }
}

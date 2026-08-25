// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.TextControl;
using JetBrains.Util;

namespace Rider.Plugins.CodonPlugin;

[ContextAction(
    Name = "GenerateBinaryCodec",
    Description = "Generates binary codec definition",
    GroupType = typeof(CSharpContextActions),
    Disabled = false,
    Priority = 1)]
public class GenerateBinaryCodecAction(ICSharpContextActionDataProvider provider) : ContextActionBase
{
    private readonly CodecGeneratorActionBase generatorBase = new CodecGeneratorActionBase(provider, CodecGeneratorActionBase.CodecKind.Binary);

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) => generatorBase.ExecutePsiTransaction(solution, progress);

    public override string Text => "Generate Binary Codec";

    public override bool IsAvailable(IUserDataHolder cache) => generatorBase.IsAvailable(cache);
}

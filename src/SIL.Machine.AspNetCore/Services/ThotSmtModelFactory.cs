﻿namespace SIL.Machine.AspNetCore.Services;

public class ThotSmtModelFactory : ISmtModelFactory
{
    private readonly IOptionsMonitor<ThotSmtModelOptions> _options;
    private readonly IOptionsMonitor<SmtTransferEngineOptions> _engineOptions;

    public ThotSmtModelFactory(
        IOptionsMonitor<ThotSmtModelOptions> options,
        IOptionsMonitor<SmtTransferEngineOptions> engineOptions
    )
    {
        _options = options;
        _engineOptions = engineOptions;
    }

    public IInteractiveTranslationModel Create(
        string engineId,
        IRangeTokenizer<string, int, string> tokenizer,
        IDetokenizer<string, string> detokenizer,
        ITruecaser truecaser
    )
    {
        string smtConfigFileName = Path.Combine(_engineOptions.CurrentValue.EnginesDir, engineId, "smt.cfg");
        var model = new ThotSmtModel(ThotWordAlignmentModelType.Hmm, smtConfigFileName)
        {
            SourceTokenizer = tokenizer,
            TargetTokenizer = tokenizer,
            TargetDetokenizer = detokenizer,
            LowercaseSource = true,
            LowercaseTarget = true,
            Truecaser = truecaser
        };
        return model;
    }

    public ITrainer CreateTrainer(
        string engineId,
        IRangeTokenizer<string, int, string> tokenizer,
        IParallelTextCorpus corpus
    )
    {
        string smtConfigFileName = Path.Combine(_engineOptions.CurrentValue.EnginesDir, engineId, "smt.cfg");
        return new ThotSmtModelTrainer(ThotWordAlignmentModelType.Hmm, corpus, smtConfigFileName)
        {
            SourceTokenizer = tokenizer,
            TargetTokenizer = tokenizer,
            LowercaseSource = true,
            LowercaseTarget = true
        };
    }

    public void InitNew(string engineId)
    {
        string engineDir = Path.Combine(_engineOptions.CurrentValue.EnginesDir, engineId);
        if (!Directory.Exists(engineDir))
            Directory.CreateDirectory(engineDir);
        ZipFile.ExtractToDirectory(_options.CurrentValue.NewModelFile, engineDir);
    }

    public void Cleanup(string engineId)
    {
        string engineDir = Path.Combine(_engineOptions.CurrentValue.EnginesDir, engineId);
        if (!Directory.Exists(engineDir))
            return;
        string lmDir = Path.Combine(engineDir, "lm");
        if (Directory.Exists(lmDir))
            Directory.Delete(lmDir, true);
        string tmDir = Path.Combine(engineDir, "tm");
        if (Directory.Exists(tmDir))
            Directory.Delete(tmDir, true);
        string smtConfigFileName = Path.Combine(engineDir, "smt.cfg");
        if (File.Exists(smtConfigFileName))
            File.Delete(smtConfigFileName);
        if (!Directory.EnumerateFileSystemEntries(engineDir).Any())
            Directory.Delete(engineDir);
    }
}

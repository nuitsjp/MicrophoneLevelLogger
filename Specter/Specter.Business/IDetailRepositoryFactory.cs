﻿using System.IO;

namespace Specter.Business;

/// <summary>
/// IDetailWriterファクトリー
/// </summary>
public interface IDetailRepositoryFactory
{
    /// <summary>
    /// IDetailWriterを生成する。
    /// </summary>
    /// <param name="writer"></param>
    /// <returns></returns>
    IDetailRepository Create(StreamWriter writer);
}
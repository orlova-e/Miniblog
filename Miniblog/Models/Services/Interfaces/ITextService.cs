﻿namespace Miniblog.Models.Services.Interfaces
{
    public interface ITextService
    {
        string GetEncoded(string text);
        object GetEncoded(object obj);
    }
}

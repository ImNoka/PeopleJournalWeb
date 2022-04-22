﻿namespace PeopleJournalWeb.Service.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        string filePath;

        public FileLoggerProvider(string path)
        {
            filePath = path;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(filePath);
        }

        public void Dispose()
        {
            
        }
    }
}

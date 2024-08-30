using System.Collections.Generic;

namespace XOR.Services
{
    public interface IProgram
    {
        Dictionary<string, Statement> Statements { get; }
        Statement GetStatement(string guid);
        void AddStatement(Statement statement);
        void RemoveStatement(string guid);
        string GetLocalPath(string path);
    }
}
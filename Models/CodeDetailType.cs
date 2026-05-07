namespace BreakpointAssistant.Models
{
    /// <summary>
    /// Defines the types of code elements that can be located.
    /// </summary>
    public enum CodeDetailType
    {
        Method,
        PublicMethod,
        PrivateMethod,
        MethodClosingBrace,
        Setter,
        Getter,
        Property,
        VariableAssignment,
        If,
        IfElse,
        Else,
        Switch,
        Case,
        For,
        Foreach,
        TryCatch,
        Try,
        Catch,
        Finally,
        Throw,
        While,
        DoWhile,
        Break,
        Continue,
        Return,
        Await,
        Constructor,
        ILogger,
        Lambda,
        Linq,
        New,
        Null,
        CustomText,
        MethodDownstream
    }
}

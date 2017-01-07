namespace VisualMutator.Extensibility
{
    public interface IMutationOperator
    {
        OperatorInfo Info { get; }

        IOperatorCodeVisitor CreateVisitor();

        IOperatorCodeRewriter CreateRewriter();
    }
}
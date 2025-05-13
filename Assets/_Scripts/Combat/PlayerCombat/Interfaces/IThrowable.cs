public interface IThrowable
{
    bool Recallable { get; }  
    bool Released { get; }
    void Throw(bool isHeavyAimAttack);            
    void Recall();
}

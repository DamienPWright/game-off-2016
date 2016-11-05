public interface IControllableActor
{
    void Jump();
    void Jump_release();
    void Attack();
    void Attack_release();
    void Up_pressed();
    void Up_released();
    void Down_pressed();
    void Down_released();
    void Forward_pressed();
    void Forward_released();
    void Special_pressed();
    void Special_released();
    void Move(float axis);
}

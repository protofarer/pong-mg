using Entity;

public static class Physics {
  public static bool HaveCollided(Paddle paddle, Ball ball) {
    if ( 
      paddle.origin.X > ball.origin.X + ball.R * 2
      || paddle.origin.X + Paddle.WIDTH < ball.origin.X
      || paddle.origin.Y > ball.origin.Y + ball.R * 2
      || paddle.origin.Y + Paddle.HEIGHT < ball.origin.Y
    )
      return false;
    return true;
  }
}
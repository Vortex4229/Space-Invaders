using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Space_Invaders;

public class Explosion {
	private readonly Vector2 _position;
	private readonly Texture2D[] _spriteArray;
	private readonly int _startFrame;
	private int _spriteIndex;

	public Explosion(Main root, Vector2 position, int startFrame) {
		_position = position;
		_startFrame = startFrame;
		var sprite1 = root.Content.Load<Texture2D>("explosion1");
		var sprite2 = root.Content.Load<Texture2D>("explosion2");
		var sprite3 = root.Content.Load<Texture2D>("explosion3");
		_spriteArray = [sprite1, sprite2, sprite3];
		Finished = false;
	}

	public bool Finished { get; private set; }

	public void Update(int frames) {
		if (frames == _startFrame + 5)
			_spriteIndex++;
		else if (frames == _startFrame + 10)
			_spriteIndex++;
		else if (frames == _startFrame + 15) Finished = true;
	}

	public void Draw(SpriteBatch spriteBatch) {
		spriteBatch.Draw(_spriteArray[_spriteIndex], _position, Color.White);
	}
}
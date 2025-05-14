using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Space_Invaders;

public class Laser {
	private readonly Main _root;
	private readonly int _spriteHeight;
	private readonly int _spriteWidth;
	private readonly int _type;
	private Vector2 _position;
	private Texture2D _sprite;


	public Laser(Main root, Vector2 position, int type) {
		_root = root;
		_type = type;
		_position = position;
		_spriteWidth = 5;
		_spriteHeight = 23;
		Hitbox = new Rectangle((int)_position.X, (int)_position.Y, _spriteWidth, _spriteHeight);
		LoadContent();
	}

	public Rectangle Hitbox { get; private set; }


	public Vector2 Location => _position;

	private void LoadContent() {
		_sprite = _root.Content.Load<Texture2D>("laserSprite");
	}

	public void Update(GameTime gameTime) {
		switch (_type) {
			case 1:
				_position.Y -= 12;
				break;
			case 2:
				_position.Y += 12;
				break;
		}

		Hitbox = new Rectangle((int)_position.X, (int)_position.Y, _spriteWidth, _spriteHeight);
	}


	public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
		spriteBatch.Draw(_sprite, _position, Color.White);
	}
}
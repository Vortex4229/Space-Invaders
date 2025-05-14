using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Space_Invaders;

public class Shield {
	private readonly Vector2 _position;
	private readonly Texture2D[] _spriteArray;
	private readonly float _spriteScale;
	private Texture2D _activeSprite;

	public Shield(Main root, Vector2 position, char type = 'e') {
		// accepted types: a b c d e (in char form)
		HealthPoints = 4;
		_position = position;
		Texture2D sprite1, sprite2, sprite3, sprite4;
		if (type != 'e') {
			sprite1 = root.Content.Load<Texture2D>($"cornerShieldBlock1{type}");
			sprite2 = root.Content.Load<Texture2D>($"cornerShieldBlock2{type}");
			sprite3 = root.Content.Load<Texture2D>($"cornerShieldBlock3{type}");
			sprite4 = root.Content.Load<Texture2D>($"cornerShieldBlock4{type}");
		}
		else if (type == 'e') {
			sprite1 = root.Content.Load<Texture2D>("squareShieldBlock1");
			sprite2 = root.Content.Load<Texture2D>("squareShieldBlock2");
			sprite3 = root.Content.Load<Texture2D>("squareShieldBlock3");
			sprite4 = root.Content.Load<Texture2D>("squareShieldBlock4");
		}
		else {
			throw new ArgumentException($"Invalid _type: {type}");
		}

		_spriteArray = [sprite4, sprite3, sprite2, sprite1];
		_activeSprite = _spriteArray[HealthPoints - 1];
		_spriteScale = 0.8f;
		var spriteWidth = _activeSprite.Width * _spriteScale;
		var spriteHeight = _activeSprite.Height * _spriteScale;
		Hitbox = new Rectangle((int)_position.X, (int)_position.Y, (int)spriteWidth, (int)spriteHeight);
	}

	public Rectangle Hitbox { get; }
	public byte HealthPoints { get; private set; }

	public void Damage() {
		HealthPoints--;
		if (HealthPoints != 0) _activeSprite = _spriteArray[HealthPoints - 1];
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
		spriteBatch.Draw(_activeSprite, _position, null, Color.White, 0f, Vector2.Zero, _spriteScale,
			SpriteEffects.None, 0f);
	}
}
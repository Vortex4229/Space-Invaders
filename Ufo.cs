using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Space_Invaders;

public class Ufo {
	private readonly Random _rnd;
	private readonly Texture2D _sprite;
	private Vector2 _position;
	private float _spriteHeight;
	private readonly float _spriteScale;
	private float _spriteWidth;

	public Ufo(Main root) {
		_sprite = root.Content.Load<Texture2D>("enemySprite4");
		_spriteScale = 1.3f;
		_spriteHeight = _sprite.Height * _spriteScale;
		_spriteWidth = _sprite.Width * _spriteScale;
		_position = new Vector2(-_spriteWidth, 10);
		Hitbox = new Rectangle((int)_position.X, (int)_position.Y, (int)_spriteWidth, (int)_spriteHeight);
		_rnd = new Random();
		IsDead = false;
	}

	public Vector2 Position => _position;

	public Rectangle Hitbox { get; private set; }

	public bool IsDead { get; private set; }

	public void Update(GameTime gameTime) {
		_position.X += 3;
		_spriteHeight = _sprite.Height * _spriteScale;
		_spriteWidth = _sprite.Width * _spriteScale;
		Hitbox = new Rectangle((int)_position.X, (int)_position.Y, (int)_spriteWidth, (int)_spriteHeight);
	}

	public int GetScore() {
		IsDead = true;
		int[] possibleScores = { 50, 100, 150, 200, 300 };
		var scoreAddress = _rnd.Next(0, possibleScores.Length);
		return possibleScores[scoreAddress];
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
		spriteBatch.Draw(_sprite, _position, null, Color.White, 0f, Vector2.Zero, _spriteScale, SpriteEffects.None, 0f);
	}
}
namespace BrickAndMortar.Content.Dusts
{
	public class RewardDust : ModDust
	{
		public override string Texture => "BrickAndMortar/Assets/Misc/GlowSoft";

		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 64, 64);

			dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(BrickAndMortar.instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{
			if (dust.fadeIn == 0)
			{
				dust.position -= Vector2.One * 32 * dust.scale;
				dust.fadeIn = 1;
			}

			Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;
			Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;

			dust.rotation += 0.06f;
			dust.position += currentCenter - nextCenter;

			dust.shader.UseColor(dust.color);

			if (dust.customData is Player)
			{
				var player = dust.customData as Player;
				dust.velocity += Vector2.Normalize(player.Center - dust.position) * 0.5f;

				if (dust.velocity.Length() > 20)
					dust.velocity = Vector2.Normalize(dust.velocity) * 19.99f;

				float dist = Vector2.Distance(player.Center, dust.position);

				if (dist < 60)
				{
					dust.velocity *= 0.85f;
					dust.scale *= 0.95f;
				}
			}

			dust.position += dust.velocity;
			dust.fadeIn -= 0.001f;
			dust.velocity *= 0.95f;

			if (!dust.noLight)
				Lighting.AddLight(dust.position, dust.color.ToVector3());

			if (dust.fadeIn < 0.1f || dust.scale < 0.1f)
				dust.active = false;

			return false;
		}
	}
}

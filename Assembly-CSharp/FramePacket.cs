using System;

[Serializable]
public class FramePacket
{
	public byte type;

	public MovePacket move;

	public SkillPacket skill;

	public UnknownSkillPacket unknown;

	public GiveUpPacket giveup;

	public PlanetBomb bomb;

	public DriftEffect effect;

	public PlanetAttack attack;

	public AddAttack addattack;

	public GunturretAttack gunattack;

	public LasergunAttack laserattack;

	public TwistAttack twistattack;

	public PlanetTeam team;
}

using System;
using NetMessage;
using Plugin;
using Solarmax;

public interface IBattleController : Lifecycle2
{
	void Reset();

	void OnRecievedFramePacket(SCFrame frame);

	void OnRecievedScriptFrame(PbSCFrames frame);

	void OnRunFramePacket(FrameNode frameNode);

	void OnPlayerMove(Node from, Node to);

	void PlayerGiveUp();

	void OnPlayerGiveUp(TEAM giveUpTeam);

	void OnPlayerDirectQuit(TEAM team);
}

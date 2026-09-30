package custom.Vanity;

import java.util.Collections;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ScheduledFuture;

import org.l2jmobius.commons.threads.ThreadPool;
import org.l2jmobius.gameserver.handler.IVoicedCommandHandler;
import org.l2jmobius.gameserver.handler.VoicedCommandHandler;
import org.l2jmobius.gameserver.model.World;
import org.l2jmobius.gameserver.model.actor.Creature;
import org.l2jmobius.gameserver.model.actor.Player;
import org.l2jmobius.gameserver.model.skill.AbnormalVisualEffect;

/**
 * Novelty chat commands that dress up everyone around you.
 * <ul>
 * <li>.onfire - everyone nearby appears to be burning.</li>
 * <li>.bighead - everyone nearby gets a comically large head.</li>
 * </ul>
 * Each command is a toggle. While it is on, a small task keeps marking creatures that come into range and unmarks them
 * when they leave, so the effect follows you around. Turning it off (or logging out) clears everything it marked.
 * <p>
 * The effects are the game's own visual ones and change nothing about combat. They live in the packets that describe a
 * creature, so everyone who can see that creature sees them too.
 */
public class VanityCommands implements IVoicedCommandHandler
{
	private static final String[] COMMANDS =
	{
		"onfire",
		"bighead",
	};

	/** How far around the player creatures are dressed up. About a screen and a half. */
	private static final int RANGE = 3000;

	/** How often creatures that walked into range are caught up with. */
	private static final long REFRESH_MILLIS = 3000;

	/** What each command shows. */
	private static final Map<String, AbnormalVisualEffect> EFFECTS = Map.of(
		"onfire", AbnormalVisualEffect.DOT_FIRE,
		"bighead", AbnormalVisualEffect.BIG_HEAD);

	/** Per player, per command: the creatures this script marked, so only those are ever unmarked. */
	private final Map<Integer, Map<String, Set<Creature>>> _marked = new ConcurrentHashMap<>();
	private final Map<Integer, ScheduledFuture<?>> _tasks = new ConcurrentHashMap<>();

	private VanityCommands()
	{
		VoicedCommandHandler.getInstance().registerHandler(this);
	}

	@Override
	public boolean onCommand(String command, Player player, String params)
	{
		final AbnormalVisualEffect effect = EFFECTS.get(command);
		if ((effect == null) || (player == null))
		{
			return false;
		}

		final Map<String, Set<Creature>> mine = _marked.computeIfAbsent(player.getObjectId(), id -> new ConcurrentHashMap<>());
		if (mine.containsKey(command))
		{
			clear(player, command);
			player.sendMessage("Everyone around you looks normal again.");
			return true;
		}

		mine.put(command, Collections.newSetFromMap(new ConcurrentHashMap<>()));
		dress(player);
		_tasks.computeIfAbsent(player.getObjectId(),
			id -> ThreadPool.scheduleAtFixedRate(() -> dress(player), REFRESH_MILLIS, REFRESH_MILLIS));
		player.sendMessage(command.equals("onfire")
			? "Everyone around you is on fire. Type .onfire again to put them out."
			: "Everyone around you has a big head. Type .bighead again to undo it.");
		return true;
	}

	/** Marks creatures now in range, and unmarks the ones that have left it. */
	private void dress(Player player)
	{
		final Map<String, Set<Creature>> mine = _marked.get(player.getObjectId());
		if ((mine == null) || mine.isEmpty())
		{
			return;
		}
		// The player went away: take everything back off and stop.
		if (!player.isOnline() || (World.getInstance().getPlayer(player.getObjectId()) == null))
		{
			for (String command : mine.keySet().toArray(new String[0]))
			{
				clear(player, command);
			}
			return;
		}

		for (Map.Entry<String, Set<Creature>> entry : mine.entrySet())
		{
			final AbnormalVisualEffect effect = EFFECTS.get(entry.getKey());
			final Set<Creature> marked = entry.getValue();

			// Anything that wandered off, or died, gets its own look back.
			for (Creature creature : marked.toArray(new Creature[0]))
			{
				if (creature.isDead() || !creature.isInsideRadius3D(player, RANGE))
				{
					creature.stopAbnormalVisualEffect(true, effect);
					marked.remove(creature);
				}
			}

			World.getInstance().forEachVisibleObjectInRange(player, Creature.class, RANGE, creature ->
			{
				// Never touch something that is legitimately showing this effect already.
				if (!creature.isDead() && !marked.contains(creature) && !creature.hasAbnormalVisualEffect(effect))
				{
					creature.startAbnormalVisualEffect(true, effect);
					marked.add(creature);
				}
			});
		}
	}

	/** Takes one command's effect back off everything it was put on. */
	private void clear(Player player, String command)
	{
		final Map<String, Set<Creature>> mine = _marked.get(player.getObjectId());
		if (mine == null)
		{
			return;
		}
		final Set<Creature> marked = mine.remove(command);
		final AbnormalVisualEffect effect = EFFECTS.get(command);
		if ((marked != null) && (effect != null))
		{
			for (Creature creature : marked)
			{
				creature.stopAbnormalVisualEffect(true, effect);
			}
		}
		if (mine.isEmpty())
		{
			_marked.remove(player.getObjectId());
			final ScheduledFuture<?> task = _tasks.remove(player.getObjectId());
			if (task != null)
			{
				task.cancel(false);
			}
		}
	}

	@Override
	public String[] getCommandList()
	{
		return COMMANDS;
	}

	public static void main(String[] args)
	{
		new VanityCommands();
	}
}

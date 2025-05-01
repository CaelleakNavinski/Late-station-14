## Vampire Matriarch
roles-antag-vamp-mat-name = Vampire Matriarch
roles-antag-vamp-mat-objective = Your objective is to lead the vampiric brood: convert enough crew and eliminate all who would refuse the blessing.

vamp-mat-role-greeting =
    You are the Vampire Matriarch.
    All of the station’s blood flows through you, and your progeny obey your command.
    Use your bite to sire others in darkness, but beware the Chaplain’s holy weapons and garlic.
    May your night reign eternal.

vamp-mat-briefing =
    Begin by converting key officers, then wash the station in crimson.
    Ensure no threat remains to unmake you or your fledgling vampires.

## Vampires
roles-antag-vamp-name = Vampire
roles-antag-vamp-objective = Your objective is to serve the Matriarch: spread your newfound blessing and protect the Matriarch at all costs.

vamp-break-control = A surge of holy light erupts as the mercy stake pierces {$name}, erasing their vampiric curse!

vamp-role-greeting =
    You are a Vampire.
    You owe fealty to the Matriarch for this blessing, and your hunger drives you to spread it.
    Convert the living to join the legion, and guard your Matriarch from holy aggression.
    Embrace the night.

vamp-briefing = Aid the Matriarch in spreading vampirism: convert the crew or spill their blood.

## General
vamp-title = Vampires
vamp-description = A stealthy nocturnal threat is stalking the station's cooridoors...

vamp-not-enough-ready-players = Not enough players readied up for the game. There were {$readyPlayersCount} out of {$minimumPlayers} required.  
vamp-no-one-ready = No players readied up! Cannot begin the Vampire invasion.
vamp-no-matriarch = There was no Vampire Matriarch to be selected. Cannot start the Vampire invasion.

## Outcomes
vamp-won = The vampires have consumed or converted enough crew members. Night claims all.  
vamp-lost = The Matriarch has fallen and the remaining brood were destroyed in the dawn’s fire.
vamp-stalemate = Both vampires and crew lie dead in pools of blood. No victors this night.  
vamp-draw = Neither side remains to claim victory.

## Matriarch Count
vamp-mat-count = The Vampire Matriarch selected:

vamp-mat-name-user = [color=#a000a0]{$name}[/color] ([color=gray]{$username}[/color]) sired {$count} {$count ->
    [one] vampire
    *[other] vampires
}
// 1. Declare external functions at the top
EXTERNAL CastSpell(spellName)
EXTERNAL ChangeSprite(spriteName)

// 2. Start the story
-> GolemEntrance

=== GolemEntrance ===
// Call the sprite change immediately
~ ChangeSprite("sick") 
The Golem stumbles in. Magma drips from his chin.
He speaks with a grinding sound.
"It burns... I can't stop working or I'll turn to stone."
    + [Cast Stasis (Ice)] 
        ~ CastSpell("ice")
        ~ ChangeSprite("frozen")
        -> GolemBad
    + [Cast Ventilation (Wind)] 
        ~ CastSpell("wind")
        ~ ChangeSprite("cured")
        -> GolemGood

=== GolemGood ===
The magma cools to a gentle obsidian.
"I can breathe again..."
-> END

=== GolemBad ===
The Golem freezes solid. He is safe, but silent.
-> END
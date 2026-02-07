// Define external functions for Unity to read
EXTERNAL Spawn(name)
EXTERNAL ChangeState(state)
EXTERNAL PlaySound(soundName)

// Variables to track game state
VAR skeleton_status = "unknown"
VAR mushroom_status = "unknown"

-> Intro

=== Intro ===
# background:clinic_morning
# audio:bell_chime
System: The Arcane Ward is open.
System: You are the Shift Wizard. Your job is to heal, not just fix.
System: Dr. Barker (that's you) adjusts his wizard hat.
+ [Call the first patient] 
    -> Skeleton_Entrance

// ---------------------------------------------------------
// PATIENT 1: THE SKELETON (BURNOUT)
// Time Estimate: 1:30
// ---------------------------------------------------------

=== Skeleton_Entrance ===
# spawn:skeleton
# audio:bones_rattling
System: A Skeleton stumbles in. He is vibrating. Smoke rises from his joints.
Skeleton: "D-doc... I need a stamina potion. Just a small one."
Skeleton: "I have three raids to lead, a dungeon to build, and... and my femur is cracking."
+ [Ask him to sit down]
    Skeleton: "I CAN'T SIT! If I sit, the guilt sets in. I have to keep moving."
    Skeleton: "Just cast *Haste* on me and I'll go."
    
    ++ [Diagnose: He is overheating.] -> Skeleton_Diagnosis

=== Skeleton_Diagnosis ===
System: You inspect his soul. It is bright orange. Overheated.
System: He doesn't need speed. He needs to stop.
Skeleton: "Doc, please! I'm falling behind!"

// THE CHOICE
+ [Choice A: Cast 'Calcify' (Force him to hold it together)]
    -> Skeleton_MiniGame_Bad
+ [Choice B: Cast 'Ventilation' (Release the pressure)]
    -> Skeleton_MiniGame_Good

// --- MINI GAME SECTION ---

=== Skeleton_MiniGame_Bad ===
System: [MINIGAME START: DRAW A SQUARE]
System: You trace the Rune of Stasis to harden his bones.
// In Unity, you will trigger the drawing game here.
// If Win -> Go to Success. If Lose -> Go to Fail.
-> Skeleton_Outcome_Bad

=== Skeleton_MiniGame_Good ===
System: [MINIGAME START: DRAW A SPIRAL]
System: You trace the Rune of Air to cool his core.
-> Skeleton_Outcome_Good

// --- OUTCOMES ---

=== Skeleton_Outcome_Bad ===
# audio:ice_crack
~ ChangeState("frozen")
System: The smoke stops instantly. The Skeleton freezes in place.
Skeleton: "Oh. Wow. I feel... solid. Unbreakable."
Skeleton: "I don't feel tired anymore. I don't feel anything."
Skeleton: "Back to work."
System: He marches out mechanically. He is efficient, but he is no longer alive.
-> Mushroom_Entrance

=== Skeleton_Outcome_Good ===
# audio:steam_hiss
~ ChangeState("cured")
System: A massive hiss of steam escapes his ribcage. He collapses into the chair.
Skeleton: "..."
Skeleton: "I... I think I just fell asleep for a second."
Skeleton: "It's quiet. The panic is gone."
Skeleton: "Maybe... maybe I'll take the day off."
System: He walks out slowly, humming a tune.
-> Mushroom_Entrance

// ---------------------------------------------------------
// PATIENT 2: THE MUSHROOM (TOXIC POSITIVITY)
// Time Estimate: 1:30
// ---------------------------------------------------------

=== Mushroom_Entrance ===
# spawn:swampy
# audio:squish_step
System: A small Mushroom Person hops in. He is smiling wide.
Mushroom: "Good morning Dr. Barker! Isn't it a lovely day!"
Mushroom: "I'm just here for a checkup! Everything is perfect!"
+ [Point out the green slime leaking from his ear]
    Mushroom: "Oh, that? Just a little happy-leak! Nothing to worry about!"
    Mushroom: "I just focus on the good vibes!"
    
    ++ [Diagnose: He is rotting from the inside.] -> Mushroom_Diagnosis

=== Mushroom_Diagnosis ===
System: You inspect his soul. It is dark sludge.
System: He is repressing so much sadness that it is physically melting him.
Mushroom: "I just need a 'Sparkle Spell' to clean this slime up! Can you do that?"

// THE CHOICE
+ [Choice A: Cast 'Polish' (Make him look pretty)]
    -> Mushroom_MiniGame_Bad
+ [Choice B: Cast 'Prune' (Cut out the rot)]
    -> Mushroom_MiniGame_Good

// --- MINI GAME SECTION ---

=== Mushroom_MiniGame_Bad ===
System: [MINIGAME START: DRAW A STAR]
System: You trace the Rune of Glamour to hide the mess.
-> Mushroom_Outcome_Bad

=== Mushroom_MiniGame_Good ===
System: [MINIGAME START: DRAW A JAGGED LINE]
System: You trace the Rune of Truth. It will hurt, but it will heal.
-> Mushroom_Outcome_Good

// --- OUTCOMES ---

=== Mushroom_Outcome_Bad ===
# audio:sparkle_chime
~ ChangeState("cured") 
// Note: We use "cured" sprite here because he looks happy, but the text reveals the horror.
System: He sparkles brightly. The slime is hidden under a layer of glitter.
Mushroom: "Yay! Look at me! I'm perfect again!"
Mushroom: "I'll go show everyone how happy I am!"
System: As he leaves, you see a trail of slime behind him. He hasn't changed. He will dissolve soon.
-> Outro

=== Mushroom_Outcome_Good ===
# audio:squish_pop
~ ChangeState("frozen")
// Note: We use "frozen" sprite (Blue/Sad) to represent him finally crying.
System: You cut the repression. The smile drops.
Mushroom: "Ouch! That... that isn't very positive!"
Mushroom: "I... I..."
Mushroom: "I'm actually really lonely, Doctor."
System: He starts to cry. The slime turns into clear water.
Mushroom: "It feels good to say that. Thank you."
-> Outro

// ---------------------------------------------------------
// OUTRO (THE PITCH)
// Time Estimate: 0:30
// ---------------------------------------------------------

=== Outro ===
System: Shift Complete.
System: The Ward is empty.
System: You hang up your Wizard Hat.
System: Sometimes magic isn't about sparkles. Sometimes it's about listening.
-> END
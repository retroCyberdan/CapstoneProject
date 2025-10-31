EXTERNAL hasItem(itemID)
EXTERNAL hasFlag(flagName)

VAR bambola_bianca_lasciata = false

// Se hai già lasciato la bambola bianca
{ bambola_bianca_lasciata:
    Hai lasciato la bambola bianca sull'altare.
    L'altare continua a brillare di una luce fredda.
    -> END
}

// Primo incontro
Sembra che si possa appoggiare qualcosa su questo altare...

{ hasItem("doll_1"):
    Vuoi appoggiare la bambola bianca?
    * [Sì]
        -> appoggia_bambola
    * [No]
        Forse più tardi.
        -> END
- else:
    -> END
}

=== appoggia_bambola ===
Appoggi delicatamente la bambola bianca sull'altare... #rimuovi_oggetto:doll_1
~ bambola_bianca_lasciata = true
L'altare si illumina di una luce fredda.

// Controlla se l'altra bambola è stata POSIZIONATA (non solo se non ce l'hai)
{ hasFlag("doll_2_posizionato"):
    Le due bambole risuonano insieme... #attiva_shifting
    Gli altari iniziano a muoversi!
}
-> END
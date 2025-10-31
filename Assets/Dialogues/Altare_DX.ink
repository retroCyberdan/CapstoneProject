EXTERNAL hasItem(itemID)
EXTERNAL hasFlag(flagName)

VAR bambola_rossa_lasciata = false

// Se hai già lasciato la bambola rossa
{ bambola_rossa_lasciata:
    Hai lasciato la bambola rossa sull'altare.
    L'altare continua a brillare di una luce fredda.
    -> END
}

// Primo incontro
Sembra che si possa appoggiare qualcosa su questo altare...

{ hasItem("doll_2"):
    Vuoi appoggiare la bambola rossa?
    * [Sì]
        -> appoggia_bambola
    * [No]
        Forse più tardi.
        -> END
- else:
    -> END
}

=== appoggia_bambola ===
Appoggi delicatamente la bambola rossa sull'altare... #rimuovi_oggetto:doll_2
~ bambola_rossa_lasciata = true
L'altare si illumina di una luce fredda.

// Controlla se l'altra bambola è stata POSIZIONATA (non solo se non ce l'hai)
{ hasFlag("doll_1_posizionato"):
    Le due bambole risuonano insieme... #attiva_shifting
    Gli altari iniziano a muoversi!
}
-> END
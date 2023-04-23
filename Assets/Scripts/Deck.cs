using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;

    public int[] values = new int[52];
    int cardIndex = 0;

    bool primeramano = true;
    public Text textoPuntosDealer;
    public Text textoPuntosPlayer;

    private void Awake()
    {
        InitCardValues();

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        int pos = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                if (j >= 10)
                {
                    values[pos] = 10;
                }
                else
                {
                    values[pos] = j + 1;
                }
                pos++;
            }
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

        int randpos = 0;
        System.Random random = new System.Random();
        for (int t=51; t>0; t--)
        {
            randpos = random.Next(t+1);

            Sprite cardface = faces[randpos];
            faces[randpos] = faces[t];
            faces[t] = cardface;

            int cardvalue = values[randpos];
            values[randpos] = values[t];
            values[t] = cardvalue;

        }
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
            if (dealer.GetComponent<CardHand>().points == 21)
            {
                finalMessage.text = "Blackjack from dealer";
                hitButton.interactable = false;
                stickButton.interactable = false;

            } else if (player.GetComponent<CardHand>().points == 21)
            {
                finalMessage.text = "Blackjack from player";
                hitButton.interactable = false;
                stickButton.interactable = false;
            }
        }
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
        textoPuntosPlayer.text = "Tus puntos: " + player.GetComponent<CardHand>().points;
    }

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        /*if (primeramano)
        {
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
            primeramano = false;
        }*/

        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (player.GetComponent<CardHand>().points>21)
        {
            finalMessage.text = "Perdiste";
            hitButton.interactable = false;
            stickButton.interactable = false;

            textoPuntosDealer.text = "Puntos: " + dealer.GetComponent<CardHand>().points;
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        }
    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        if (primeramano)
        {
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
            primeramano = false;
        }

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */

        if (dealer.GetComponent<CardHand>().points <= 16)
        {
            PushDealer();
            Stand();
        }
        else
        {
            int puntosdealer = dealer.GetComponent<CardHand>().points;
            int puntosplayer = player.GetComponent<CardHand>().points;
            if(puntosdealer > puntosplayer && puntosdealer<=21)
            {
                finalMessage.text = "Gana el dealer";
                hitButton.interactable = false;
                stickButton.interactable = false;
            }
            else if (puntosdealer > 21)
            {
                finalMessage.text = "Gana el jugador";
                hitButton.interactable = false;
                stickButton.interactable = false;
            } 
            else if(puntosdealer < puntosplayer)
            {
                finalMessage.text = "Gana el jugador";
                hitButton.interactable = false;
                stickButton.interactable = false;
            } else if(puntosdealer == puntosplayer)
            {
                finalMessage.text = "Empate";
                hitButton.interactable = false;
                stickButton.interactable = false;
            }
            textoPuntosDealer.text = "Puntos: " + dealer.GetComponent<CardHand>().points;
        }

    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        textoPuntosDealer.text = "Puntos:";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
}
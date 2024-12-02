// Uniform Cost Path Finding for map 1

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UniformCostSearch : MonoBehaviour
{
    Grid grid; // Grid sınıfına ait bir nesne
    Player player; // Player sınıfına ait bir nesne
    public Transform seeker, target; // Seeker ve hedef Transform bileşenleri
    public bool driveable = true; // Oyuncunun hareket edebilir durumda olup olmadığını kontrol eder

    private void Awake()
    {
        grid = GetComponent<Grid>(); // Bu GameObject'teki Grid bileşenini al
        player = FindObjectOfType<Player>(); // Haritadaki Player objesini bul
    }

    private void Update()
    {
        FindPath(seeker.position, target.position); // Seeker ve hedef arasındaki yolu bul
        GoToTarget(); // Hedefe gitmek için işlemleri başlat
    }

    void GoToTarget()
    {
        // Eğer bir yol varsa ve oyuncu hareket edebiliyorsa
        if (grid.path1 != null && grid.path1.Count > 0 && driveable)
        {
            Vector3 hedefNokta = grid.path1[0].WorldPosition; // Yolun ilk düğümünün konumunu al
            player.LookToTarget(hedefNokta); // Oyuncuyu hedefe doğru döndür
            player.GidilcekYer(hedefNokta); // Oyuncunun pozisyonunu hedefe doğru güncelle
        }
    }

    void FindPath(Vector3 startPoz, Vector3 targetPoz)
    {
        // startNode ve targetNode düğümleri oluştur
        Node startNode = grid.NodeFromWorldPoint(startPoz); // Başlangıç pozisyonundan node'u bul
        Node targetNode = grid.NodeFromWorldPoint(targetPoz); // Hedef pozisyonundan node'u bul

        PriorityQueue<Node> openSet = new PriorityQueue<Node>(); // Kontrol edilecek düğümleri saklamak için bir öncelik kuyruğu
        HashSet<Node> closedSet = new HashSet<Node>(); // Zaten kontrol edilmiş düğümleri saklamak için bir set

        startNode.gCost = 0; // Başlangıç düğümünün maliyetini sıfır olarak ayarla
        openSet.Enqueue(startNode, startNode.gCost); // Başlangıç düğümünü öncelik kuyruğuna ekle

        while (openSet.Count > 0) // Açık küme boş olmadığı sürece döngü devam eder
        {
            Node currentNode = openSet.Dequeue(); // Öncelik kuyruğundan en düşük maliyetli düğümü al

            // Eğer hedefe ulaşıldıysa yolu geri izleyerek oluştur
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode); // Yolu oluştur
                return;
            }

            closedSet.Add(currentNode); // Şu anki düğümü kapalı kümeye ekle

            foreach (Node neighbour in grid.GetNeighbours(currentNode)) // Şu anki düğümün tüm komşularını kontrol et
            {
                // Eğer komşu düğüm geçilebilir değilse veya kapalı kümede ise devam et
                if (!neighbour.Walkable || closedSet.Contains(neighbour))
                    continue;

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour); // Komşu düğümün yeni maliyetini hesapla

                // Eğer komşu düğüm açık kümede yoksa veya yeni maliyet daha düşükse
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour; // Yeni maliyeti ata
                    neighbour.parent = currentNode; // Komşunun ebeveynini güncelle

                    if (!openSet.Contains(neighbour))
                        openSet.Enqueue(neighbour, neighbour.gCost); // Komşuyu öncelik kuyruğuna ekle
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>(); // Oluşturulan yolu saklamak için bir liste
        Node currentNode = endNode; // Yolu hedef düğümden başlat

        while (currentNode != startNode) // Başlangıç düğümüne ulaşana kadar geri izle
        {
            path.Add(currentNode); // Şu anki düğümü yola ekle
            currentNode = currentNode.parent; // Bir önceki düğüme geç
        }

        path.Reverse(); // Listeyi ters çevir (başlangıçtan hedefe doğru)
        grid.path1 = path; // Grid'deki path1 değişkenine oluşturulan yolu ata
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX); // X koordinatlarındaki farkı al
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY); // Y koordinatlarındaki farkı al

        return 10 * (dstX + dstY); // Manhattan mesafesini hesapla (grid tabanlı yollar için uygun)
    }
}



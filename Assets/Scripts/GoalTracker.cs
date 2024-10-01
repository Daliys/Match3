using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalTracker : MonoBehaviour
{
    [SerializeField] private TMP_Text firstGoalText;
    [SerializeField] private TileType firstGoalType;
    [SerializeField] private TMP_Text secondGoalText;
    [SerializeField] private TileType secondGoalType;

    [SerializeField] private TMP_Text totalScoreText;

    [SerializeField] private Image firstGoalImage; // Первая картинка для первой цели
    [SerializeField] private Image secondGoalImage;

    [SerializeField] private int firstGoal;
    [SerializeField] private int secondGoal;

    private int _firstGoal;
    private int _secondGoal;
    private int _totalScore;

    // Ссылки на активные твины для предотвращения наложения
    private Tweener _firstGoalTweener;
    private Tweener _secondGoalTweener;
    private Tweener _totalScoreTweener;


    private void Start()
    {
        _firstGoal = firstGoal;
        _secondGoal = secondGoal;
        _totalScore = 0;

        firstGoalText.text = _firstGoal.ToString();
        secondGoalText.text = _secondGoal.ToString();
        totalScoreText.text = _totalScore.ToString();
    }



    public void OnGemsCollected(TileType type, int count)
    {
        if (type == firstGoalType)
        {
            _firstGoal -= count;
            _firstGoal = Mathf.Max(0, _firstGoal);
            AnimateText(firstGoalText, _firstGoal + count, _firstGoal, ref _firstGoalTweener);
            if (_firstGoal <= 0)
            {
                HandleGoalCompletion(firstGoalText, firstGoalImage);
            }
        }
        else if (type == secondGoalType)
        {
            _secondGoal -= count;
            _secondGoal = Mathf.Max(0, _secondGoal);
            AnimateText(secondGoalText, _secondGoal + count, _secondGoal, ref _secondGoalTweener);
            if (_secondGoal <= 0)
            {
                HandleGoalCompletion(secondGoalText, secondGoalImage);
            }
        }

        _totalScore += count * 10;
        AnimateText(totalScoreText, _totalScore - count * 10, _totalScore, ref _totalScoreTweener);

    }

    private void HandleGoalCompletion(TMP_Text text, Image image)
    {
        // Создаем последовательность анимаций
        Sequence seq = DOTween.Sequence();

        // Эффект вращения и масштабирования
        seq.Join(text.transform.DORotate(new Vector3(0, 0, 360), 0.60f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine));
        seq.Join(text.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack));
        seq.Join(text.DOFade(0f, 0.5f).SetEase(Ease.InBack));

        // Эффект вращения и масштабирования изображения
        seq.Join(image.transform.DORotate(new Vector3(0, 0, -360), 0.6f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine));
        seq.Join(image.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack));
        seq.Join(image.DOFade(0f, 0.5f).SetEase(Ease.InBack));

        // Добавление эффекта частиц или взрыва (опционально)
        // Здесь можно запустить систему частиц или анимацию взрыва

        // После завершения анимации, скрываем объекты
        seq.OnComplete(() =>
        {
            text.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
        });
    }


    private void AnimateText(TMP_Text text, int startValue, int endValue, ref Tweener tweener)
    {
        // Если уже существует активный твин для этого текста, остановить его
        if (tweener != null && tweener.IsActive())
        {
            tweener.Kill();
        }

        // Создаем твин для анимации числового значения
        tweener = DOTween.To(() => startValue, x => UpdateTextValue(text, x), endValue, 0.3f)
                       .SetEase(Ease.Linear)
                       .OnStart(() => ApplyTextEffects(text, true))
                       .OnComplete(() => ApplyTextEffects(text, false));
    }



    private void UpdateTextValue(TMP_Text text, int value)
    {
        text.text = value.ToString();
    }


    private void ApplyTextEffects(TMP_Text text, bool isAnimating)
    {
        if (isAnimating)
        {
            // Масштабирование текста для эффекта "прыжка"
            text.transform.DOScale(1.2f, 0.15f).SetEase(Ease.OutBack);
        }
        else
        {
            // Возвращение масштаба к исходному состоянию
            text.transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack);
        }
    }

}
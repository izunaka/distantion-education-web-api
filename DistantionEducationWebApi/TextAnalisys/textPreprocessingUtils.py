import pymorphy3 as morph
from nltk.corpus import stopwords
from wiki_ru_wordnet import WikiWordnet
import nltk
morphAnalyzer = morph.MorphAnalyzer()
wordnet = WikiWordnet()
russianStopwords = stopwords.words('russian')
from math import sqrt
import argparse

def processText(text):
    textLowerCase = text.lower().replace('\n', ' ').replace('\r', ' ')
    onlyWordsText = deleteInsignificantChars(textLowerCase)
    words = list(filter(lambda word: word != '', onlyWordsText.split(' ')))
    clearWords = deleteStopWords(words)
    wordsInitialForms = getWordsInitialForms(clearWords)
    tokens = getTokens(wordsInitialForms)
    return tokens

def correlativeProcessTexts(text1, text2, maxFine):
    tokens1 = processText(text1)
    tokens2 = processText(text2)
    tokensKeys1 = tokens1.keys()
    tokensKeys2 = set(tokens2.keys())
    replacedTokensCount = 0
    allTokensCount = 0
    replacements = {}
    for token in tokensKeys2:
        allTokensCount += tokens2[token]
    for word in tokensKeys1:
        synonyms = wordnet.get_synsets(word)
        synonymsSet = set()
        for item in synonyms:
            for synonym in item.get_words():
                if (synonym.lemma() != word):
                    synonymsSet.add(synonym.lemma())
        intersection = tokensKeys2.intersection(synonymsSet)
        tokensCount = 0
        for synonym in intersection:
            if (synonym in tokens2.keys()):
                tokensCount += tokens2[synonym]
                tokens2.pop(synonym)
                replacements[synonym] = word
        if(tokensCount > 0):
            tokens2[word] = tokensCount
            replacedTokensCount += tokensCount
    return tokens1, tokens2, maxFine * replacedTokensCount / allTokensCount, replacements
        


def deleteInsignificantChars(text):
    onlyWords = ''
    for char in text:
        if (str.isalpha(char) or char == ' '):
            onlyWords = onlyWords + char
    return onlyWords

def deleteStopWords(words):
    result = []
    for word in words:
        if (not word in russianStopwords):
            result.append(word)
    return result

def getWordInitialForm(word):
    return morphAnalyzer.parse(word)[0].normal_form

def getWordsInitialForms(words):
    result = []
    for word in words:
        result.append(getWordInitialForm(word))
    return result

def getTokens(words):
    tokens = dict()
    for word in words:
        token = tokens.get(word)
        tokens[word] = 1 if token == None else token + 1
    return tokens

def getVectorsCos(vec1, vec2):
    multi = 0; len1 = 0; len2 = 0
    for i in range(0, len(vec1)):
        multi += vec1[i] * vec2[i]
        len1 += vec1[i] ** 2
        len2 += vec2[i] ** 2
    return multi / (sqrt(len1) * sqrt(len2))

def postprocessTexts(tokens1, tokens2, text1, text2, replacements):
    tokens1 = tokens1.keys(); tokens2 = tokens2.keys()
    tokensIntersection = set(tokens1).intersection(set(tokens2))
    words1 = deleteStopWords(deleteInsignificantChars(text1.lower()).split(' '))
    words2 = deleteStopWords(deleteInsignificantChars(text2.lower()).split(' '))
    generalTerminsInFirstText = list(filter(lambda word: getWordInitialForm(word) in tokensIntersection, words1))
    extraTerminsInFirstText = list(filter(lambda word: getWordInitialForm(word) not in tokensIntersection, words1))
    generalTerminsInSecondText = list(filter(lambda word: getWordInitialFormWithReaplcements(word, replacements) in tokensIntersection, words2))
    extraTerminsInSecondText = list(filter(lambda word: getWordInitialFormWithReaplcements(word, replacements) not in tokensIntersection, words2))
    return generalTerminsInFirstText, extraTerminsInFirstText, generalTerminsInSecondText, extraTerminsInSecondText

def getWordInitialFormWithReaplcements(word, replacements):
    initialForm = getWordInitialForm(word)
    return replacements[initialForm] if initialForm in replacements.keys() else initialForm

def getInputParams(argv):
    # text1 = open('text1').read()
    # text2 = open('text2').read()
    parser = argparse.ArgumentParser()
    parser.add_argument('-t1', '--text1', default='', type=str)
    parser.add_argument('-t2', '--text2', default='', type=str)
    parser.add_argument('-s', '--synonyms', default=True, type=bool)
    parser.add_argument('-m', '--maxfine', default=0, type=float)
    parser.add_argument('-f', '--frequency', default=False, type=bool)
    return parser.parse_args(argv[1:])
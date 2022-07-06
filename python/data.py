import pandas as pd
import json
import os
import re
import random
from sklearn.preprocessing import LabelEncoder


def get_vectors():
    #directory = 'C:\\Users\\Iga\\Desktop\\IgAgent\\games'
    directory = 'C:\\Users\\Iga\\Desktop\\game'

    all_games = []
    all_games_moves = []
    for filename in os.listdir(directory):
        filepath = os.path.join(directory, filename)
        with open(filepath) as f:
            fdata = f.read()
            try:
                json_game = json.loads(fdata)
            except:
                print(filename)
        all_games_moves.append(json_game['moves'])
        all_games.append(json_game)

    action_types = []
    actions = []

    for i in range(0, len(all_games)):
        current_game_moves = all_games[i]['moves']
        for j in range(0, len(current_game_moves)):
            action_type = current_game_moves[j]['actionType']
            action = current_game_moves[j]['action']
            if 'Play Card' in action_type:
                action_type = 'Play Card'
            if action_type == 'End Turn':
                action = 'End Turn'
                current_game_moves[j]['turn'] -= 1
            if action_type == 'Hero Power':
                action = 'Hero Power'
            action_type_stripped = re.sub("\\[[^>]+\\]", "", str(action_type))
            action_stripped = re.sub("\\[[^>]+\\]", "", str(action))
            current_game_moves[j]['actionType'] = action_type_stripped
            current_game_moves[j]['action'] = action_stripped
            action_types.append(action_type_stripped)
            actions.append(action_stripped)
        all_games[i]['moves'] = current_game_moves

    players_dict, action_types_dict, actions_dict = get_dicts(action_types, actions)

    games_vec = []
    results_vec = []

    for i in range(0, len(all_games)):
        game_vec = []
        max_turn = all_games[i]['moves'][-1]['turn']
        cutoff = random.randint(1, max_turn)
        for move in all_games[i]['moves']:
            if move['turn'] <= cutoff:
                player = move['player']
                action_type = move['actionType']
                action = move['action']
                game_vec.append(players_dict[player])
                game_vec.append(action_types_dict[action_type])
                game_vec.append(actions_dict[action])
        games_vec.append(game_vec)

    for i in range(0, len(all_games)):
        result = all_games[i]['result']
        results_vec.append(result)

    max_len = 0
    for game_vec in games_vec:
        vec_len = len(game_vec)
        if vec_len > max_len:
            max_len = vec_len

    for i in range(0, len(games_vec)):
        vec = games_vec[i]
        if len(vec) != max_len:
            diff = max_len - len(vec)
            for j in range(0, diff):
                vec.append(0)
        games_vec[i] = vec

    # open file in write mode
    # with open(r'./x_values.txt', 'w') as x:
    #     for game_vec in games_vec:
    #         # write each item on a new line
    #         x.write("%s\n" % game_vec)
    #
    # with open(r'./y_values.txt', 'w') as y:
    #     for result_vec in results_vec:
    #         # write each item on a new line
    #         y.write("%s\n" % result_vec)

    print(games_vec)
    return games_vec, results_vec


def get_dicts(action_types, actions):
    action_types_categories = set(action_types)
    actions_categories = set(actions)

    label_encoder = LabelEncoder()

    action_types_df = pd.DataFrame(action_types_categories, columns=['Action_Type'])
    action_types_df['Action_Type_Cat'] = label_encoder.fit_transform(action_types_df['Action_Type'])
    action_types_df['Action_Type_Cat'] += 100

    actions_df = pd.DataFrame(actions_categories, columns=['Action'])
    actions_df['Action_Cat'] = label_encoder.fit_transform(actions_df['Action'])
    actions_df['Action_Cat'] += 1000

    players_dict = {}
    action_types_dict = {}
    actions_dict = {}

    players_dict[1] = 2
    players_dict[2] = 4

    for ind in action_types_df.index:
        key = action_types_df['Action_Type'][ind]
        value = action_types_df['Action_Type_Cat'][ind]
        action_types_dict[key] = value

    for ind in actions_df.index:
        key = actions_df['Action'][ind]
        value = actions_df['Action_Cat'][ind]
        actions_dict[key] = value

    return players_dict, action_types_dict, actions_dict


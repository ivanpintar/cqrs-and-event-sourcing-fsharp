import { actionTypes } from './actions';
import Basket from './models/basketRecord';

export const basket = (state, action) => {
    switch (action.type) {
        case actionTypes.UPDATE_BASKET:
            return new Basket(action.basket);
        default:
            return state;
    }
};
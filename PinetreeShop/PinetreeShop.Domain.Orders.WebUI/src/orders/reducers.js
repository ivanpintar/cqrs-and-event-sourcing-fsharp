import { actionTypes } from './actions';
import Immutable from 'immutable';
import Order from './models/orderRecord';

const order = (state, action) => {
    switch (action.type) {
        case actionTypes.ADD_OR_UPDATE_ORDER:
            return new Order(action.order);
        default:
            return state;
    }
};

export const orders = (state = Immutable.List(), action) => {
    switch (action.type) {
        case actionTypes.ADD_OR_UPDATE_ORDER:
            if (state.some(o => o.id === action.order.id)) {
                return state.map(o => o.id === action.order.id ? order(o, action) : o);
            } 
            return state.push(order(undefined, action));
        default:
            return state;
    }
};
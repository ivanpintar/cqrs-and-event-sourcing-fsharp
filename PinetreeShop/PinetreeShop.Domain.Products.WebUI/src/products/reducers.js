import { actionTypes } from './actions';
import Immutable from 'immutable';
import Product from './models/productRecord';

const product = (state, action) => {
    switch (action.type) {
        case actionTypes.ADD_OR_UPDATE_PRODUCT:
            return new Product(action.product);
        case actionTypes.CHANGE_PRODUCT_QUANTITY:
            if(state.id !== action.id) return state;
            return state.set('quantity', action.quantity);
        default:
            return state;
    }
};

export const products = (state = Immutable.List(), action) => {
    switch (action.type) {
        case actionTypes.ADD_OR_UPDATE_PRODUCT:
            if (state.some(p => p.id === action.product.id)) {
                return state.map(p => p.id === action.product.id ? product(p, action) : p);
            } 
            return state.push(product(undefined, action));
        case actionTypes.CHANGE_PRODUCT_QUANTITY:
            return state.map(p => product(p, action));
        default:
            return state;
    }
};
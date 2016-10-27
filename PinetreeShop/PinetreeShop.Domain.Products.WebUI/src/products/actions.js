import { toastr } from 'react-redux-toastr';

export const actionTypes = {
    ADD_PRODUCT: 'ADD_PRODUCT',
    SET_PRODUCT_QUANTITY: 'SET_PRODUCT_QUANTITY',
    FILTER_PRODUCTS: 'FILTER_PRODUCTS'
}

export const addProduct = (name, price) => {
    return dispatch => {
        dispatch({
            type: actionTypes.ADD_PRODUCT,
            name,
            price
        });
    };
}

export const setProductQuantity = (id, quantity) => {
    return dispatch => {
        dispatch({
            type: actionTypes.SET_PRODUCT_QUANTITY,
            id,
            quantity
        });
        toastr.success('Quantity changed');
    };
}

export const filterProducts = (filterText) => {
    return dispatch => {
        dispatch({
            type: actionTypes.FILTER_PRODUCTS,
            filterText
        });
    };
}
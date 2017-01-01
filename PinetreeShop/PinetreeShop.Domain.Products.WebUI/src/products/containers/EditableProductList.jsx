import { connect } from 'react-redux';
import { addProduct, changeProductQuantity } from '../actions';
import ProductList from '../components/ProductList';

const sortProducts = (a,b) => {
    if (a.name < b.name) return -1; 
    if (a.name > b.name) return 1;
    return 0; 
}

const getSortedProducts = (products) => {
    return products
        .sort(sortProducts);    
}

const mapStateToProps = (state) => {
    return {
        products: getSortedProducts(state.products)
    };
}

const mapDispatchToProps = (dispatch) => {
    return {
        addProduct: (name, price, quantity) => {
            dispatch(addProduct(name, price, quantity));
        },
        changeQuantity: (id, diff) => {
            dispatch(changeProductQuantity(id, diff));
        }
    };
}

const EditableProductList = connect(mapStateToProps, mapDispatchToProps)(ProductList);

export default EditableProductList;
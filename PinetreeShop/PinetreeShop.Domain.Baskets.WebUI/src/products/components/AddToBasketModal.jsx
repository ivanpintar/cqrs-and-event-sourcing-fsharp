import React from 'react';
import { FormControl } from 'react-bootstrap'
import ModalDialog from '../../global/components/ModalDialog';

class AddToBasketModal extends React.Component{
    constructor(props) {
        super(props);
        this.handleChange = this.handleChange.bind(this);
        this.submit = this.submit.bind(this);
        this.close = this.close.bind(this);
        this.state = {
            quantity: 1
        }; 
    }

     componentWillReceiveProps(props) {
        this.setState({ 
            quantity: 1
        }); 
    }

    handleChange(e) {
        this.setState({ quantity: e.target.value });
    }

    close() {
        this.setState({
            quantity: 0
        });
        this.props.onClose();
    }

    submit() {    
        if(this.props.product && this.state.quantity) {
            this.props.addToBasket(this.props.product.id, this.state.quantity);
        }
        this.props.onClose();    
    }

    render() {
        let title = '';
        if(this.props.product) {
            title = 'Add ' + this.props.product.name + ' to basket'; 
        }
        return (
            <ModalDialog 
                show={this.props.show}
                onClose={this.props.onClose}
                onSave={this.submit}
                title={title}>
                <FormControl
                    value={this.state.quantity}
                    type="text"
                    onChange={this.handleChange}/>     
            </ModalDialog>
        );
    }
}

export default AddToBasketModal;